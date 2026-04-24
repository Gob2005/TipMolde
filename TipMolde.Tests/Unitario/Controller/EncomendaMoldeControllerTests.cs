using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TipMolde.API.Controllers;
using TipMolde.Application.Dtos.EncomendaMoldeDto;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Comercio.IEncomendaMolde;

namespace TipMolde.Tests.Unitario.Controller;

[TestFixture]
[Category("Unit")]
public class EncomendaMoldeControllerTests
{
    private Mock<IEncomendaMoldeService> _service = null!;
    private Mock<ILogger<EncomendaMoldeController>> _logger = null!;
    private EncomendaMoldeController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _service = new Mock<IEncomendaMoldeService>();
        _logger = new Mock<ILogger<EncomendaMoldeController>>();

        _controller = new EncomendaMoldeController(_service.Object, _logger.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };
    }

    [Test(Description = "TENCMCONT1 - Get por encomenda deve devolver bad request quando page e invalida.")]
    public async Task GetByEncomendaId_Should_ReturnBadRequest_When_PageInvalid()
    {
        // ARRANGE

        // ACT
        var result = await _controller.GetByEncomendaId(1, 0, 10);

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test(Description = "TENCMCONT2 - Create deve devolver created at action quando pedido e valido.")]
    public async Task Create_Should_ReturnCreatedAtAction_When_RequestIsValid()
    {
        // ARRANGE
        var dto = new CreateEncomendaMoldeDto
        {
            Encomenda_id = 1,
            Molde_id = 2,
            Quantidade = 10,
            Prioridade = 1,
            DataEntregaPrevista = DateTime.UtcNow.AddDays(5)
        };

        var response = new ResponseEncomendaMoldeDto
        {
            EncomendaMolde_id = 99,
            Encomenda_id = 1,
            Molde_id = 2,
            Quantidade = 10,
            Prioridade = 1,
            DataEntregaPrevista = dto.DataEntregaPrevista
        };

        _service.Setup(s => s.CreateAsync(dto)).ReturnsAsync(response);

        // ACT
        var result = await _controller.Create(dto);

        // ASSERT
        result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Test(Description = "TENCMCONT3 - GetById deve devolver not found quando associacao nao existe.")]
    public async Task GetById_Should_ReturnNotFound_When_LinkDoesNotExist()
    {
        // ARRANGE
        _service.Setup(s => s.GetByIdAsync(44)).ReturnsAsync((ResponseEncomendaMoldeDto?)null);

        // ACT
        var result = await _controller.GetById(44);

        // ASSERT
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test(Description = "TENCMCONT4 - GetById deve devolver dto quando associacao existe.")]
    public async Task GetById_Should_ReturnOk_When_LinkExists()
    {
        // ARRANGE
        var response = new ResponseEncomendaMoldeDto
        {
            EncomendaMolde_id = 11,
            Encomenda_id = 1,
            Molde_id = 2,
            Quantidade = 10,
            Prioridade = 1,
            DataEntregaPrevista = new DateTime(2026, 5, 1)
        };
        _service.Setup(s => s.GetByIdAsync(11)).ReturnsAsync(response);

        // ACT
        var result = await _controller.GetById(11);

        // ASSERT
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(response);
    }

    [Test(Description = "TENCMCONT5 - Get por encomenda deve devolver payload paginado quando pedido e valido.")]
    public async Task GetByEncomendaId_Should_ReturnOk_When_RequestIsValid()
    {
        // ARRANGE
        var paged = new PagedResult<ResponseEncomendaMoldeDto>(
            new[]
            {
                new ResponseEncomendaMoldeDto { EncomendaMolde_id = 1, Encomenda_id = 3, Molde_id = 4, Quantidade = 5, Prioridade = 1 }
            },
            1,
            1,
            10);

        _service.Setup(s => s.GetByEncomendaIdAsync(3, 1, 10)).ReturnsAsync(paged);

        // ACT
        var result = await _controller.GetByEncomendaId(3, 1, 10);

        // ASSERT
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(paged);
    }

    [Test(Description = "TENCMCONT6 - Get por molde deve devolver bad request quando pageSize e invalido.")]
    public async Task GetByMoldeId_Should_ReturnBadRequest_When_PaginationIsInvalid()
    {
        // ACT
        var result = await _controller.GetByMoldeId(4, 1, 0);

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test(Description = "TENCMCONT7 - Get por molde deve devolver payload paginado quando pedido e valido.")]
    public async Task GetByMoldeId_Should_ReturnOk_When_RequestIsValid()
    {
        // ARRANGE
        var paged = new PagedResult<ResponseEncomendaMoldeDto>(
            new[]
            {
                new ResponseEncomendaMoldeDto { EncomendaMolde_id = 2, Encomenda_id = 3, Molde_id = 4, Quantidade = 7, Prioridade = 2 }
            },
            1,
            2,
            5);

        _service.Setup(s => s.GetByMoldeIdAsync(4, 2, 5)).ReturnsAsync(paged);

        // ACT
        var result = await _controller.GetByMoldeId(4, 2, 5);

        // ASSERT
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(paged);
    }

    [Test(Description = "TENCMCONT8 - Create deve devolver bad request quando model state e invalido.")]
    public async Task Create_Should_ReturnBadRequest_When_ModelStateIsInvalid()
    {
        // ARRANGE
        _controller.ModelState.AddModelError("Quantidade", "Obrigatorio");

        // ACT
        var result = await _controller.Create(new CreateEncomendaMoldeDto());

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
        _service.Verify(s => s.CreateAsync(It.IsAny<CreateEncomendaMoldeDto>()), Times.Never);
    }

    [Test(Description = "TENCMCONT9 - Update deve devolver bad request quando model state e invalido.")]
    public async Task Update_Should_ReturnBadRequest_When_ModelStateIsInvalid()
    {
        // ARRANGE
        _controller.ModelState.AddModelError("Quantidade", "Obrigatorio");

        // ACT
        var result = await _controller.Update(9, new UpdateEncomendaMoldeDto { Quantidade = 12 });

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
        _service.Verify(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<UpdateEncomendaMoldeDto>()), Times.Never);
    }

    [Test(Description = "TENCMCONT10 - Update deve devolver no content quando pedido e valido.")]
    public async Task Update_Should_ReturnNoContent_When_RequestIsValid()
    {
        // ARRANGE
        var dto = new UpdateEncomendaMoldeDto { Quantidade = 15, Prioridade = 3 };

        // ACT
        var result = await _controller.Update(9, dto);

        // ASSERT
        result.Should().BeOfType<NoContentResult>();
        _service.Verify(s => s.UpdateAsync(9, dto), Times.Once);
    }

    [Test(Description = "TENCMCONT11 - Delete deve devolver no content quando pedido e valido.")]
    public async Task Delete_Should_ReturnNoContent_When_RequestIsValid()
    {
        // ACT
        var result = await _controller.Delete(13);

        // ASSERT
        result.Should().BeOfType<NoContentResult>();
        _service.Verify(s => s.DeleteAsync(13), Times.Once);
    }
}
