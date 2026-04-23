using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TipMolde.API.Controllers;
using TipMolde.Application.DTOs.MoldeDTO;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Producao.IMolde;
using TipMolde.Application.Interface.Relatorios;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Unitario.Controller;

[TestFixture]
[Category("Unit")]
public class MoldeControllerTests
{
    private Mock<IMoldeService> _moldeService = null!;
    private Mock<IRelatorioService> _relatorioService = null!;
    private Mock<ILogger<MoldeController>> _logger = null!;
    private MoldeController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _moldeService = new Mock<IMoldeService>();
        _relatorioService = new Mock<IRelatorioService>();
        _logger = new Mock<ILogger<MoldeController>>();

        _controller = new MoldeController(
            _moldeService.Object,
            _relatorioService.Object,
            _logger.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    private static ResponseMoldeDTO BuildResponse(int id = 1, string numero = "MOL-001")
    {
        return new ResponseMoldeDTO
        {
            MoldeId = id,
            Numero = numero,
            NumeroMoldeCliente = "CLI-001",
            Nome = "Molde",
            ImagemCapaPath = "capa.png",
            Descricao = "Descricao",
            Numero_cavidades = 4,
            TipoPedido = TipoPedido.NOVO_MOLDE,
            Largura = 10,
            Comprimento = 20,
            Altura = 30,
            PesoEstimado = 40,
            TipoInjecao = "Hot Runner",
            SistemaInjecao = "Canal Quente",
            Contracao = 1.2m,
            AcabamentoPeca = "Polido",
            Cor = CorMolde.MONOCOLOR,
            MaterialMacho = "P20",
            MaterialCavidade = "H13",
            MaterialMovimentos = "420",
            MaterialInjecao = "ABS"
        };
    }

    [Test(Description = "TMOLDCONT1 - GetAll deve devolver bad request quando paginacao e invalida.")]
    public async Task GetAll_Should_ReturnBadRequest_When_PaginationIsInvalid()
    {
        // ARRANGE

        // ACT
        var result = await _controller.GetAll(0, 10);

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test(Description = "TMOLDCONT2 - GetById deve devolver not found quando molde nao existe.")]
    public async Task GetById_Should_ReturnNotFound_When_MoldeDoesNotExist()
    {
        // ARRANGE
        _moldeService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((ResponseMoldeDTO?)null);

        // ACT
        var result = await _controller.GetById(99);

        // ASSERT
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test(Description = "TMOLDCONT3 - GetByNumero deve devolver bad request quando numero e vazio.")]
    public async Task GetByNumero_Should_ReturnBadRequest_When_NumeroIsBlank()
    {
        // ARRANGE

        // ACT
        var result = await _controller.GetByNumero("   ");

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test(Description = "TMOLDCONT4 - Create deve devolver status 400 quando model state e invalido.")]
    public async Task Create_Should_ReturnStatus400_When_ModelStateIsInvalid()
    {
        // ARRANGE
        _controller.ModelState.AddModelError("Numero", "Obrigatorio");
        var dto = new CreateMoldeDTO
        {
            Numero = "MOL-100",
            Numero_cavidades = 4,
            TipoPedido = TipoPedido.NOVO_MOLDE,
            EncomendaId = 1,
            Quantidade = 10,
            Prioridade = 1,
            DataEntregaPrevista = DateTime.UtcNow.AddDays(5)
        };

        // ACT
        var result = await _controller.Create(dto);

        // ASSERT
        var objectResult = result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        _moldeService.Verify(s => s.CreateAsync(It.IsAny<CreateMoldeDTO>()), Times.Never);
    }

    [Test(Description = "TMOLDCONT5 - Create deve devolver created at action quando pedido e valido.")]
    public async Task Create_Should_ReturnCreatedAtAction_When_RequestIsValid()
    {
        // ARRANGE
        var dto = new CreateMoldeDTO
        {
            Numero = "MOL-200",
            Numero_cavidades = 8,
            TipoPedido = TipoPedido.NOVO_MOLDE,
            EncomendaId = 7,
            Quantidade = 25,
            Prioridade = 2,
            DataEntregaPrevista = new DateTime(2026, 5, 15)
        };
        var response = BuildResponse(id: 200, numero: "MOL-200");

        _moldeService.Setup(s => s.CreateAsync(dto)).ReturnsAsync(response);

        // ACT
        var result = await _controller.Create(dto);

        // ASSERT
        var created = result as CreatedAtActionResult;
        created.Should().NotBeNull();
        created!.ActionName.Should().Be(nameof(MoldeController.GetById));
        created.RouteValues.Should().ContainKey("id");
        created.RouteValues!["id"].Should().Be(200);
        created.Value.Should().BeEquivalentTo(response);
    }

    [Test(Description = "TMOLDCONT6 - Update deve devolver no content quando pedido e valido.")]
    public async Task Update_Should_ReturnNoContent_When_RequestIsValid()
    {
        // ARRANGE
        var dto = new UpdateMoldeDTO
        {
            Nome = "Molde Atualizado",
            MaterialInjecao = "PP"
        };

        // ACT
        var result = await _controller.Update(55, dto);

        // ASSERT
        result.Should().BeOfType<NoContentResult>();
        _moldeService.Verify(s => s.UpdateAsync(55, It.Is<UpdateMoldeDTO>(x =>
            x.Nome == "Molde Atualizado" &&
            x.MaterialInjecao == "PP")), Times.Once);
    }

    [Test(Description = "TMOLDCONT7 - Export PDF deve devolver ficheiro quando o relatorio e gerado com sucesso.")]
    public async Task ExportCicloVidaPdf_Should_ReturnFile_When_ReportIsGenerated()
    {
        // ARRANGE
        var content = new byte[] { 1, 2, 3, 4 };
        _relatorioService.Setup(s => s.GerarCicloVidaMoldePdfAsync(10))
            .ReturnsAsync((content, "molde-10.pdf"));

        // ACT
        var result = await _controller.ExportCicloVidaPdf(10);

        // ASSERT
        var file = result as FileContentResult;
        file.Should().NotBeNull();
        file!.FileContents.Should().Equal(content);
        file.ContentType.Should().Be("application/pdf");
        file.FileDownloadName.Should().Be("molde-10.pdf");
    }

    [Test(Description = "TMOLDCONT8 - GetByEncomendaId deve devolver payload paginado quando o pedido e valido.")]
    public async Task GetByEncomendaId_Should_ReturnOkWithPagedPayload_When_RequestIsValid()
    {
        // ARRANGE
        var items = new List<ResponseMoldeDTO> { BuildResponse(id: 1), BuildResponse(id: 2) };
        var paged = new PagedResult<ResponseMoldeDTO>(items, 2, 1, 10);

        _moldeService.Setup(s => s.GetByEncomendaIdAsync(7, 1, 10)).ReturnsAsync(paged);

        // ACT
        var result = await _controller.GetByEncomendaId(7);

        // ASSERT
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(paged);
    }
}
