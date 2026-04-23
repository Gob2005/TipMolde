using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TipMolde.API.Controllers;
using TipMolde.Application.DTOs.ProjetoDTO;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Desenho.IProjeto;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Unitario.Controller;

[TestFixture]
public class ProjetoControllerTests
{
    private Mock<IProjetoService> _projetoService = null!;
    private Mock<ILogger<ProjetoController>> _logger = null!;
    private ProjetoController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _projetoService = new Mock<IProjetoService>();
        _logger = new Mock<ILogger<ProjetoController>>();

        _controller = new ProjetoController(
            _projetoService.Object,
            _logger.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    private static ResponseProjetoDTO BuildResponse(int id = 1)
    {
        return new ResponseProjetoDTO
        {
            Projeto_id = id,
            NomeProjeto = "Projeto",
            SoftwareUtilizado = "SolidWorks",
            TipoProjeto = TipoProjeto.PROJETO_3D,
            CaminhoPastaServidor = @"\\srv\projetos\proj-1",
            Molde_id = 7
        };
    }

    [Test(Description = "TPROJCONT1 - GetAll deve devolver bad request quando paginacao e invalida.")]
    public async Task GetAll_Should_ReturnBadRequest_When_PaginationIsInvalid()
    {
        // ARRANGE

        // ACT
        var result = await _controller.GetAll(0, 10);

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test(Description = "TPROJCONT2 - GetById deve devolver not found quando projeto nao existe.")]
    public async Task GetById_Should_ReturnNotFound_When_ProjetoDoesNotExist()
    {
        // ARRANGE
        _projetoService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((ResponseProjetoDTO?)null);

        // ACT
        var result = await _controller.GetById(99);

        // ASSERT
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test(Description = "TPROJCONT3 - Create deve devolver status 400 quando model state e invalido.")]
    public async Task Create_Should_ReturnStatus400_When_ModelStateIsInvalid()
    {
        // ARRANGE
        _controller.ModelState.AddModelError("NomeProjeto", "Obrigatorio");

        var dto = new CreateProjetoDTO
        {
            NomeProjeto = "Projeto X",
            SoftwareUtilizado = "AutoCAD",
            TipoProjeto = TipoProjeto.PROJETO_2D,
            CaminhoPastaServidor = @"\\srv\projetos\proj-x",
            Molde_id = 1
        };

        // ACT
        var result = await _controller.Create(dto);

        // ASSERT
        var objectResult = result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        _projetoService.Verify(s => s.CreateAsync(It.IsAny<CreateProjetoDTO>()), Times.Never);
    }

    [Test(Description = "TPROJCONT4 - Create deve devolver created at action quando pedido e valido.")]
    public async Task Create_Should_ReturnCreatedAtAction_When_RequestIsValid()
    {
        // ARRANGE
        var dto = new CreateProjetoDTO
        {
            NomeProjeto = "Projeto 200",
            SoftwareUtilizado = "NX",
            TipoProjeto = TipoProjeto.PROJETO_3D,
            CaminhoPastaServidor = @"\\srv\projetos\proj-200",
            Molde_id = 7
        };

        var response = BuildResponse(id: 200);

        _projetoService.Setup(s => s.CreateAsync(dto)).ReturnsAsync(response);

        // ACT
        var result = await _controller.Create(dto);

        // ASSERT
        var created = result as CreatedAtActionResult;
        created.Should().NotBeNull();
        created!.ActionName.Should().Be(nameof(ProjetoController.GetById));
        created.RouteValues.Should().ContainKey("id");
        created.RouteValues!["id"].Should().Be(200);
        created.Value.Should().BeEquivalentTo(response);
    }

    [Test(Description = "TPROJCONT5 - Update deve devolver no content quando pedido e valido.")]
    public async Task Update_Should_ReturnNoContent_When_RequestIsValid()
    {
        // ARRANGE
        var dto = new UpdateProjetoDTO
        {
            NomeProjeto = "Projeto Atualizado"
        };

        // ACT
        var result = await _controller.Update(55, dto);

        // ASSERT
        result.Should().BeOfType<NoContentResult>();
        _projetoService.Verify(s => s.UpdateAsync(55, It.Is<UpdateProjetoDTO>(x =>
            x.NomeProjeto == "Projeto Atualizado")), Times.Once);
    }

    [Test(Description = "TPROJCONT6 - GetWithRevisoes deve devolver payload enriquecido quando projeto existe.")]
    public async Task GetWithRevisoes_Should_ReturnOk_When_ProjetoExists()
    {
        // ARRANGE
        var response = new ResponseProjetoWithRevisoesDTO
        {
            Projeto_id = 10,
            NomeProjeto = "Projeto 10",
            SoftwareUtilizado = "SolidWorks",
            TipoProjeto = TipoProjeto.PROJETO_3D,
            CaminhoPastaServidor = @"\\srv\projetos\proj-10",
            Molde_id = 4,
            Revisoes = new List<TipMolde.Application.DTOs.RevisaoDTO.ResponseRevisaoDTO>
            {
                new() { Revisao_id = 1, NumRevisao = 2, DescricaoAlteracoes = "Rev 2", Projeto_id = 10 }
            }
        };

        _projetoService.Setup(s => s.GetWithRevisoesAsync(10)).ReturnsAsync(response);

        // ACT
        var result = await _controller.GetWithRevisoes(10);

        // ASSERT
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(response);
    }

    [Test(Description = "TPROJCONT7 - GetAll deve devolver payload paginado quando o pedido e valido.")]
    public async Task GetAll_Should_ReturnOkWithPagedPayload_When_RequestIsValid()
    {
        // ARRANGE
        var items = new List<ResponseProjetoDTO> { BuildResponse(1), BuildResponse(2) };
        var paged = new PagedResult<ResponseProjetoDTO>(items, 2, 1, 10);

        _projetoService.Setup(s => s.GetAllAsync(1, 10)).ReturnsAsync(paged);

        // ACT
        var result = await _controller.GetAll(1, 10);

        // ASSERT
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(paged);
    }
}
