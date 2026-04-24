using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TipMolde.API.Controllers;
using TipMolde.Application.Dtos.FasesProducaoDto;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Producao.IFasesProducao;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Unitario.Controller;

[TestFixture]
[Category("Unit")]
public class FasesProducaoControllerTests
{
    private Mock<IFasesProducaoService> _service = null!;
    private Mock<ILogger<FasesProducaoController>> _logger = null!;
    private FasesProducaoController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _service = new Mock<IFasesProducaoService>();
        _logger = new Mock<ILogger<FasesProducaoController>>();

        _controller = new FasesProducaoController(_service.Object, _logger.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    private static ResponseFasesProducaoDto BuildResponse(int id = 1, Nome_fases nome = Nome_fases.MAQUINACAO)
    {
        return new ResponseFasesProducaoDto
        {
            FasesProducao_id = id,
            Nome = nome,
            Descricao = "Descricao"
        };
    }

    [Test(Description = "TFPCONT1 - GetAll deve devolver bad request quando a paginacao e invalida.")]
    public async Task GetAll_Should_ReturnBadRequest_When_PaginationIsInvalid()
    {
        // ARRANGE

        // ACT
        var result = await _controller.GetAll(0, 10);

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test(Description = "TFPCONT2 - GetById deve devolver not found quando a fase nao existe.")]
    public async Task GetById_Should_ReturnNotFound_When_FaseDoesNotExist()
    {
        // ARRANGE
        _service.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((ResponseFasesProducaoDto?)null);

        // ACT
        var result = await _controller.GetById(99);

        // ASSERT
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test(Description = "TFPCONT3 - Create deve devolver bad request quando model state e invalido.")]
    public async Task Create_Should_ReturnBadRequest_When_ModelStateIsInvalid()
    {
        // ARRANGE
        _controller.ModelState.AddModelError("Nome", "Obrigatorio");

        // ACT
        var result = await _controller.Create(new CreateFasesProducaoDto { Nome = Nome_fases.MAQUINACAO });

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
        _service.Verify(s => s.CreateAsync(It.IsAny<CreateFasesProducaoDto>()), Times.Never);
    }

    [Test(Description = "TFPCONT4 - Create deve devolver created at action quando o pedido e valido.")]
    public async Task Create_Should_ReturnCreatedAtAction_When_RequestIsValid()
    {
        // ARRANGE
        var dto = new CreateFasesProducaoDto { Nome = Nome_fases.MONTAGEM, Descricao = "Descricao" };
        var response = BuildResponse(id: 7, nome: Nome_fases.MONTAGEM);
        _service.Setup(s => s.CreateAsync(dto)).ReturnsAsync(response);

        // ACT
        var result = await _controller.Create(dto);

        // ASSERT
        var created = result as CreatedAtActionResult;
        created.Should().NotBeNull();
        created!.ActionName.Should().Be(nameof(FasesProducaoController.GetById));
        created.RouteValues!["id"].Should().Be(7);
        created.Value.Should().BeEquivalentTo(response);
    }

    [Test(Description = "TFPCONT5 - Update deve devolver no content quando o pedido e valido.")]
    public async Task Update_Should_ReturnNoContent_When_RequestIsValid()
    {
        // ARRANGE
        var dto = new UpdateFasesProducaoDto { Descricao = "Descricao atualizada" };

        // ACT
        var result = await _controller.Update(5, dto);

        // ASSERT
        result.Should().BeOfType<NoContentResult>();
        _service.Verify(s => s.UpdateAsync(5, It.Is<UpdateFasesProducaoDto>(x =>
            x.Descricao == "Descricao atualizada")), Times.Once);
    }

    [Test(Description = "TFPCONT6 - Delete deve devolver no content quando o pedido e valido.")]
    public async Task Delete_Should_ReturnNoContent_When_RequestIsValid()
    {
        // ARRANGE

        // ACT
        var result = await _controller.Delete(3);

        // ASSERT
        result.Should().BeOfType<NoContentResult>();
        _service.Verify(s => s.DeleteAsync(3), Times.Once);
    }

    [Test(Description = "TFPCONT7 - GetAll deve devolver payload paginado quando o pedido e valido.")]
    public async Task GetAll_Should_ReturnOk_When_RequestIsValid()
    {
        // ARRANGE
        var paged = new PagedResult<ResponseFasesProducaoDto>(
            new[] { BuildResponse(1, Nome_fases.MAQUINACAO), BuildResponse(2, Nome_fases.EROSAO) },
            2,
            1,
            10);

        _service.Setup(s => s.GetAllAsync(1, 10)).ReturnsAsync(paged);

        // ACT
        var result = await _controller.GetAll(1, 10);

        // ASSERT
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(paged);
    }
}
