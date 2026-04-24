using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TipMolde.API.Controllers;
using TipMolde.Application.Dtos.EncomendaDto;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Comercio.IEncomenda;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Unitario.Controller;

[TestFixture]
[Category("Unit")]
public class EncomendaControllerTests
{
    private Mock<IEncomendaService> _encomendaService = null!;
    private Mock<ILogger<EncomendaController>> _logger = null!;
    private EncomendaController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _encomendaService = new Mock<IEncomendaService>();
        _logger = new Mock<ILogger<EncomendaController>>();

        _controller = new EncomendaController(_encomendaService.Object, _logger.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    private static ResponseEncomendaDto BuildResponse(int id = 1, string numero = "ENC-001")
    {
        return new ResponseEncomendaDto
        {
            Encomenda_id = id,
            NumeroEncomendaCliente = numero,
            NumeroProjetoCliente = "PRJ-001",
            NomeServicoCliente = "Servico",
            NomeResponsavelCliente = "Maria",
            DataRegisto = new DateTime(2026, 4, 21, 8, 0, 0, DateTimeKind.Utc),
            Estado = EstadoEncomenda.CONFIRMADA,
            Cliente_id = 10,
            NomeCliente = "Cliente A"
        };
    }

    [Test(Description = "TENCCONT1 - GetAll deve devolver bad request quando paginacao e invalida.")]
    public async Task GetAllEncomendas_Should_ReturnBadRequest_When_PaginationIsInvalid()
    {
        // ARRANGE

        // ACT
        var result = await _controller.GetAllEncomendas(0, 10);

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test(Description = "TENCCONT2 - GetById deve devolver not found quando encomenda nao existe.")]
    public async Task GetEncomendaById_Should_ReturnNotFound_When_EncomendaDoesNotExist()
    {
        // ARRANGE
        _encomendaService.Setup(s => s.GetByIdAsync(88)).ReturnsAsync((ResponseEncomendaDto?)null);

        // ACT
        var result = await _controller.GetEncomendaById(88);

        // ASSERT
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test(Description = "TENCCONT3 - GetById deve devolver dto quando encomenda existe.")]
    public async Task GetEncomendaById_Should_ReturnOkWithDto_When_EncomendaExists()
    {
        // ARRANGE
        var response = BuildResponse(id: 3);
        _encomendaService.Setup(s => s.GetByIdAsync(3)).ReturnsAsync(response);

        // ACT
        var result = await _controller.GetEncomendaById(3);

        // ASSERT
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(response);
    }

    [Test(Description = "TENCCONT4 - Create deve devolver bad request quando model state e invalido.")]
    public async Task CreateEncomenda_Should_ReturnBadRequest_When_ModelStateIsInvalid()
    {
        // ARRANGE
        _controller.ModelState.AddModelError("NumeroEncomendaCliente", "Obrigatorio");
        var dto = new CreateEncomendaDto
        {
            Cliente_id = 1,
            NumeroEncomendaCliente = "ENC-1"
        };

        // ACT
        var result = await _controller.CreateEncomenda(dto);

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
        _encomendaService.Verify(s => s.CreateAsync(It.IsAny<CreateEncomendaDto>()), Times.Never);
    }

    [Test(Description = "TENCCONT5 - Create deve devolver created at action quando dados sao validos.")]
    public async Task CreateEncomenda_Should_ReturnCreatedAtAction_When_RequestIsValid()
    {
        // ARRANGE
        var dto = new CreateEncomendaDto
        {
            Cliente_id = 10,
            NumeroEncomendaCliente = "ENC-100",
            NumeroProjetoCliente = "PRJ-100",
            NomeServicoCliente = "Servico",
            NomeResponsavelCliente = "Ana"
        };
        var response = BuildResponse(id: 100, numero: "ENC-100");

        _encomendaService.Setup(s => s.CreateAsync(dto)).ReturnsAsync(response);

        // ACT
        var result = await _controller.CreateEncomenda(dto);

        // ASSERT
        var created = result as CreatedAtActionResult;
        created.Should().NotBeNull();
        created!.ActionName.Should().Be(nameof(EncomendaController.GetEncomendaById));
        created.RouteValues.Should().ContainKey("id");
        created.RouteValues!["id"].Should().Be(100);
        created.Value.Should().BeEquivalentTo(response);
    }

    [Test(Description = "TENCCONT6 - Update deve devolver no content quando pedido e valido.")]
    public async Task UpdateEncomenda_Should_ReturnNoContent_When_RequestIsValid()
    {
        // ARRANGE
        var dto = new UpdateEncomendaDto
        {
            NumeroEncomendaCliente = "ENC-500",
            NomeServicoCliente = "Servico Novo"
        };

        // ACT
        var result = await _controller.UpdateEncomenda(500, dto);

        // ASSERT
        result.Should().BeOfType<NoContentResult>();
        _encomendaService.Verify(s => s.UpdateAsync(500, It.Is<UpdateEncomendaDto>(e =>
            e.NumeroEncomendaCliente == "ENC-500" &&
            e.NomeServicoCliente == "Servico Novo")), Times.Once);
    }

    [Test(Description = "TENCCONT7 - UpdateEstado deve devolver no content quando estado e valido.")]
    public async Task UpdateEstado_Should_ReturnNoContent_When_RequestIsValid()
    {
        // ARRANGE
        var dto = new UpdateEstadoEncomendaDto { Estado = EstadoEncomenda.EM_PRODUCAO };

        // ACT
        var result = await _controller.UpdateEstado(9, dto);

        // ASSERT
        result.Should().BeOfType<NoContentResult>();
        _encomendaService.Verify(s => s.UpdateEstadoAsync(9, It.Is<UpdateEstadoEncomendaDto>(x => x.Estado == EstadoEncomenda.EM_PRODUCAO)), Times.Once);
    }

    [Test(Description = "TENCCONT8 - Delete deve devolver no content quando pedido e valido.")]
    public async Task DeleteEncomenda_Should_ReturnNoContent_When_RequestIsValid()
    {
        // ARRANGE

        // ACT
        var result = await _controller.DeleteEncomenda(12);

        // ASSERT
        result.Should().BeOfType<NoContentResult>();
        _encomendaService.Verify(s => s.DeleteAsync(12), Times.Once);
    }

    [Test(Description = "TENCCONT9 - GetAll deve devolver payload com itens paginados quando pedido e valido.")]
    public async Task GetAllEncomendas_Should_ReturnPagedPayload_When_RequestIsValid()
    {
        // ARRANGE
        var items = new List<ResponseEncomendaDto> { BuildResponse(id: 1), BuildResponse(id: 2) };
        _encomendaService.Setup(s => s.GetAllAsync(1, 10))
            .ReturnsAsync(new PagedResult<ResponseEncomendaDto>(items, 2, 1, 10));

        // ACT
        var result = await _controller.GetAllEncomendas(1, 10);

        // ASSERT
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().NotBeNull();
        var payload = ok.Value!;
        payload.GetType().GetProperty("Items").Should().NotBeNull();
    }

    [Test(Description = "TENCCONT10 - GetWithMoldes deve devolver payload quando encomenda existe.")]
    public async Task GetEncomendaWithMoldes_Should_ReturnOk_When_EncomendaExists()
    {
        // ARRANGE
        var response = BuildResponse(id: 7);
        _encomendaService.Setup(s => s.GetEncomendaWithMoldesAsync(7)).ReturnsAsync(response);

        // ACT
        var result = await _controller.GetEncomendaWithMoldes(7);

        // ASSERT
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(response);
    }

    [Test(Description = "TENCCONT11 - GetPorConcluir deve devolver bad request quando paginacao e invalida.")]
    public async Task GetEncomendasPorConcluir_Should_ReturnBadRequest_When_PaginationIsInvalid()
    {
        // ACT
        var result = await _controller.GetEncomendasPorConcluir(0, 10);

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test(Description = "TENCCONT12 - GetPorConcluir deve devolver payload paginado quando pedido e valido.")]
    public async Task GetEncomendasPorConcluir_Should_ReturnOk_When_RequestIsValid()
    {
        // ARRANGE
        var paged = new PagedResult<ResponseEncomendaDto>(new[] { BuildResponse(id: 4) }, 1, 1, 10);
        _encomendaService.Setup(s => s.GetEncomendasPorConcluirAsync(1, 10)).ReturnsAsync(paged);

        // ACT
        var result = await _controller.GetEncomendasPorConcluir(1, 10);

        // ASSERT
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(paged);
    }

    [Test(Description = "TENCCONT13 - GetByEstado deve devolver payload paginado quando pedido e valido.")]
    public async Task GetByEstado_Should_ReturnOk_When_RequestIsValid()
    {
        // ARRANGE
        var paged = new PagedResult<ResponseEncomendaDto>(new[] { BuildResponse(id: 5) }, 1, 2, 5);
        _encomendaService
            .Setup(s => s.GetByEstadoAsync(EstadoEncomenda.EM_PRODUCAO, 2, 5))
            .ReturnsAsync(paged);

        // ACT
        var result = await _controller.GetByEstado(EstadoEncomenda.EM_PRODUCAO, 2, 5);

        // ASSERT
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(paged);
    }

    [Test(Description = "TENCCONT14 - GetByNumeroCliente deve devolver bad request quando numero e vazio.")]
    public async Task GetByNumeroCliente_Should_ReturnBadRequest_When_NumeroIsBlank()
    {
        // ACT
        var result = await _controller.GetByNumeroCliente("   ");

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test(Description = "TENCCONT15 - GetByNumeroCliente deve devolver not found quando encomenda nao existe.")]
    public async Task GetByNumeroCliente_Should_ReturnNotFound_When_EncomendaDoesNotExist()
    {
        // ARRANGE
        _encomendaService
            .Setup(s => s.GetByNumeroEncomendaClienteAsync("ENC-404"))
            .ReturnsAsync((ResponseEncomendaDto?)null);

        // ACT
        var result = await _controller.GetByNumeroCliente("ENC-404");

        // ASSERT
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test(Description = "TENCCONT16 - GetByNumeroCliente deve devolver dto quando encomenda existe.")]
    public async Task GetByNumeroCliente_Should_ReturnOk_When_EncomendaExists()
    {
        // ARRANGE
        var response = BuildResponse(id: 6, numero: "ENC-006");
        _encomendaService.Setup(s => s.GetByNumeroEncomendaClienteAsync("ENC-006")).ReturnsAsync(response);

        // ACT
        var result = await _controller.GetByNumeroCliente("ENC-006");

        // ASSERT
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(response);
    }

    [Test(Description = "TENCCONT17 - Update deve devolver bad request quando model state e invalido.")]
    public async Task UpdateEncomenda_Should_ReturnBadRequest_When_ModelStateIsInvalid()
    {
        // ARRANGE
        _controller.ModelState.AddModelError("NumeroEncomendaCliente", "Obrigatorio");

        // ACT
        var result = await _controller.UpdateEncomenda(1, new UpdateEncomendaDto { NumeroEncomendaCliente = "ENC-1" });

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
        _encomendaService.Verify(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<UpdateEncomendaDto>()), Times.Never);
    }

    [Test(Description = "TENCCONT18 - UpdateEstado deve devolver bad request quando model state e invalido.")]
    public async Task UpdateEstado_Should_ReturnBadRequest_When_ModelStateIsInvalid()
    {
        // ARRANGE
        _controller.ModelState.AddModelError("Estado", "Obrigatorio");

        // ACT
        var result = await _controller.UpdateEstado(1, new UpdateEstadoEncomendaDto { Estado = EstadoEncomenda.EM_PRODUCAO });

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
        _encomendaService.Verify(s => s.UpdateEstadoAsync(It.IsAny<int>(), It.IsAny<UpdateEstadoEncomendaDto>()), Times.Never);
    }
}
