using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TipMolde.API.Controllers;
using TipMolde.Application.Dtos.PecaDto;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Producao.IPeca;

namespace TipMolde.Tests.Unitario.Controller;

[TestFixture]
[Category("Unit")]
public class PecaControllerTests
{
    private Mock<IPecaService> _service = null!;
    private Mock<ILogger<PecaController>> _logger = null!;
    private PecaController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        // ARRANGE
        _service = new Mock<IPecaService>();
        _logger = new Mock<ILogger<PecaController>>();

        _controller = new PecaController(_service.Object, _logger.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    private static ResponsePecaDto BuildResponse(int id = 1, int moldeId = 7, string designacao = "Extrator") => new()
    {
        PecaId = id,
        Designacao = designacao,
        Prioridade = 2,
        MaterialDesignacao = "Aco",
        MaterialRecebido = false,
        Molde_id = moldeId
    };

    [Test(Description = "TPECACONT1 - GetAll deve devolver bad request quando paginacao e invalida.")]
    public async Task GetAll_Should_ReturnBadRequest_When_PaginationIsInvalid()
    {
        // ARRANGE

        // ACT
        var result = await _controller.GetAll(0, 10);

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
        _service.Verify(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Test(Description = "TPECACONT2 - GetAll deve devolver payload paginado quando pedido e valido.")]
    public async Task GetAll_Should_ReturnOk_When_RequestIsValid()
    {
        // ARRANGE
        var paged = new PagedResult<ResponsePecaDto>(new[] { BuildResponse(id: 3) }, 1, 1, 10);
        _service.Setup(s => s.GetAllAsync(1, 10)).ReturnsAsync(paged);

        // ACT
        var result = await _controller.GetAll(1, 10);

        // ASSERT
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(paged);
    }

    [Test(Description = "TPECACONT3 - GetById deve devolver not found quando a peca nao existe.")]
    public async Task GetById_Should_ReturnNotFound_When_PecaDoesNotExist()
    {
        // ARRANGE
        _service.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((ResponsePecaDto?)null);

        // ACT
        var result = await _controller.GetById(99);

        // ASSERT
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test(Description = "TPECACONT4 - GetByMoldeId deve devolver payload paginado quando o pedido e valido.")]
    public async Task GetByMoldeId_Should_ReturnOkWithPagedPayload_When_RequestIsValid()
    {
        // ARRANGE
        var items = new List<ResponsePecaDto> { BuildResponse(id: 1, moldeId: 7), BuildResponse(id: 2, moldeId: 7) };
        var paged = new PagedResult<ResponsePecaDto>(items, 2, 1, 10);

        _service.Setup(s => s.GetByMoldeIdAsync(7, 1, 10)).ReturnsAsync(paged);

        // ACT
        var result = await _controller.GetByMoldeId(7, 1, 10);

        // ASSERT
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(paged);
    }

    [Test(Description = "TPECACONT5 - Create deve devolver status 400 quando model state e invalido.")]
    public async Task Create_Should_ReturnStatus400_When_ModelStateIsInvalid()
    {
        // ARRANGE
        _controller.ModelState.AddModelError("Designacao", "Obrigatorio");
        var dto = new CreatePecaDto
        {
            Designacao = "Extrator",
            Prioridade = 1,
            Molde_id = 7
        };

        // ACT
        var result = await _controller.Create(dto);

        // ASSERT
        var objectResult = result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        _service.Verify(s => s.CreateAsync(It.IsAny<CreatePecaDto>()), Times.Never);
    }

    [Test(Description = "TPECACONT6 - Create deve devolver created at action quando pedido e valido.")]
    public async Task Create_Should_ReturnCreatedAtAction_When_RequestIsValid()
    {
        // ARRANGE
        var dto = new CreatePecaDto
        {
            Designacao = "Extrator",
            Prioridade = 1,
            MaterialDesignacao = "Aco",
            MaterialRecebido = false,
            Molde_id = 7
        };
        var response = BuildResponse(id: 20, moldeId: 7);

        _service.Setup(s => s.CreateAsync(dto)).ReturnsAsync(response);

        // ACT
        var result = await _controller.Create(dto);

        // ASSERT
        var created = result as CreatedAtActionResult;
        created.Should().NotBeNull();
        created!.ActionName.Should().Be(nameof(PecaController.GetById));
        created.RouteValues!["id"].Should().Be(20);
        created.Value.Should().BeEquivalentTo(response);
    }

    [Test(Description = "TPECACONT7 - Update deve devolver no content quando pedido e valido.")]
    public async Task Update_Should_ReturnNoContent_When_RequestIsValid()
    {
        // ARRANGE
        var dto = new UpdatePecaDto
        {
            Designacao = "Nova Peca",
            MaterialRecebido = true
        };

        // ACT
        var result = await _controller.Update(55, dto);

        // ASSERT
        result.Should().BeOfType<NoContentResult>();
        _service.Verify(s => s.UpdateAsync(55, It.Is<UpdatePecaDto>(x =>
            x.Designacao == "Nova Peca" &&
            x.MaterialRecebido == true)), Times.Once);
    }
}
