using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TipMolde.API.Controllers;
using TipMolde.Application.Dtos.MaquinaDto;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Producao.IMaquina;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Unitario.Controller;

[TestFixture]
[Category("Unit")]
public class MaquinaControllerTests
{
    private Mock<IMaquinaService> _maquinaService = null!;
    private Mock<ILogger<MaquinaController>> _logger = null!;
    private MaquinaController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _maquinaService = new Mock<IMaquinaService>();
        _logger = new Mock<ILogger<MaquinaController>>();

        _controller = new MaquinaController(_maquinaService.Object, _logger.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    private static ResponseMaquinaDto BuildResponse(
        int id = 1,
        int numero = 10,
        string nomeModelo = "Makino V33",
        string? ipAddress = "192.168.1.10",
        EstadoMaquina estado = EstadoMaquina.DISPONIVEL,
        int faseDedicadaId = 5)
    {
        return new ResponseMaquinaDto
        {
            Maquina_id = id,
            Numero = numero,
            NomeModelo = nomeModelo,
            IpAddress = ipAddress,
            Estado = estado,
            FaseDedicada_id = faseDedicadaId
        };
    }

    [Test(Description = "TMAQCONT1 - GetAll deve devolver bad request quando a paginacao e invalida.")]
    public async Task GetAll_Should_ReturnBadRequest_When_PaginationIsInvalid()
    {
        // ARRANGE

        // ACT
        var result = await _controller.GetAll(0, 10);

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test(Description = "TMAQCONT2 - GetAll deve devolver payload paginado quando o pedido e valido.")]
    public async Task GetAll_Should_ReturnPagedPayload_When_RequestIsValid()
    {
        // ARRANGE
        var paged = new PagedResult<ResponseMaquinaDto>(
            new[] { BuildResponse(id: 1), BuildResponse(id: 2, numero: 20) },
            2,
            1,
            10);

        _maquinaService.Setup(s => s.GetAllAsync(1, 10))
            .ReturnsAsync(paged);

        // ACT
        var result = await _controller.GetAll(1, 10);

        // ASSERT
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(paged);
    }

    [Test(Description = "TMAQCONT3 - GetById deve devolver not found quando a maquina nao existe.")]
    public async Task GetById_Should_ReturnNotFound_When_MaquinaDoesNotExist()
    {
        // ARRANGE
        _maquinaService.Setup(s => s.GetByIdAsync(40))
            .ReturnsAsync((ResponseMaquinaDto?)null);

        // ACT
        var result = await _controller.GetById(40);

        // ASSERT
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test(Description = "TMAQCONT4 - GetById deve devolver dto quando a maquina existe.")]
    public async Task GetById_Should_ReturnOkWithDto_When_MaquinaExists()
    {
        // ARRANGE
        var response = BuildResponse(id: 3, numero: 33, estado: EstadoMaquina.EM_USO, faseDedicadaId: 7);
        _maquinaService.Setup(s => s.GetByIdAsync(3))
            .ReturnsAsync(response);

        // ACT
        var result = await _controller.GetById(3);

        // ASSERT
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(response);
    }

    [Test(Description = "TMAQCONT5 - GetByEstado deve devolver bad request quando a paginacao e invalida.")]
    public async Task GetByEstado_Should_ReturnBadRequest_When_PaginationIsInvalid()
    {
        // ACT
        var result = await _controller.GetByEstado(EstadoMaquina.MANUTENCAO, 0, 10);

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test(Description = "TMAQCONT6 - GetByEstado deve devolver payload paginado quando o pedido e valido.")]
    public async Task GetByEstado_Should_ReturnOk_When_RequestIsValid()
    {
        // ARRANGE
        var paged = new PagedResult<ResponseMaquinaDto>(
            new[] { BuildResponse(id: 5, numero: 50, estado: EstadoMaquina.MANUTENCAO) },
            1,
            2,
            5);

        _maquinaService.Setup(s => s.GetByEstadoAsync(EstadoMaquina.MANUTENCAO, 2, 5))
            .ReturnsAsync(paged);

        // ACT
        var result = await _controller.GetByEstado(EstadoMaquina.MANUTENCAO, 2, 5);

        // ASSERT
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(paged);
    }

    [Test(Description = "TMAQCONT7 - Create deve devolver bad request quando model state e invalido.")]
    public async Task Create_Should_ReturnBadRequest_When_ModelStateIsInvalid()
    {
        // ARRANGE
        _controller.ModelState.AddModelError("NomeModelo", "Obrigatorio");
        var dto = new CreateMaquinaDto
        {
            Maquina_id = 1,
            Numero = 10,
            NomeModelo = "Makino",
            Estado = EstadoMaquina.DISPONIVEL,
            FaseDedicada_id = 5
        };

        // ACT
        var result = await _controller.Create(dto);

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
        _maquinaService.Verify(s => s.CreateAsync(It.IsAny<CreateMaquinaDto>()), Times.Never);
    }

    [Test(Description = "TMAQCONT8 - Create deve devolver created at action quando os dados sao validos.")]
    public async Task Create_Should_ReturnCreatedAtAction_When_RequestIsValid()
    {
        // ARRANGE
        var dto = new CreateMaquinaDto
        {
            Maquina_id = 11,
            Numero = 101,
            NomeModelo = "Makino",
            IpAddress = "10.0.0.11",
            Estado = EstadoMaquina.DISPONIVEL,
            FaseDedicada_id = 8
        };

        var response = BuildResponse(
            id: 11,
            numero: 101,
            nomeModelo: "Makino",
            ipAddress: "10.0.0.11",
            estado: EstadoMaquina.DISPONIVEL,
            faseDedicadaId: 8);

        _maquinaService.Setup(s => s.CreateAsync(dto))
            .ReturnsAsync(response);

        // ACT
        var result = await _controller.Create(dto);

        // ASSERT
        var created = result as CreatedAtActionResult;
        created.Should().NotBeNull();
        created!.ActionName.Should().Be(nameof(MaquinaController.GetById));
        created.RouteValues!["id"].Should().Be(11);
        created.Value.Should().BeEquivalentTo(response);
    }

    [Test(Description = "TMAQCONT9 - Update deve devolver no content quando o pedido e valido.")]
    public async Task Update_Should_ReturnNoContent_When_RequestIsValid()
    {
        // ARRANGE
        var dto = new UpdateMaquinaDto
        {
            Numero = 202,
            NomeModelo = "Novo Modelo",
            IpAddress = "10.10.10.20",
            Estado = EstadoMaquina.EM_USO,
            FaseDedicada_id = 9
        };

        // ACT
        var result = await _controller.Update(20, dto);

        // ASSERT
        result.Should().BeOfType<NoContentResult>();
        _maquinaService.Verify(s => s.UpdateAsync(20, It.Is<UpdateMaquinaDto>(x =>
            x.Numero == 202 &&
            x.NomeModelo == "Novo Modelo" &&
            x.IpAddress == "10.10.10.20" &&
            x.Estado == EstadoMaquina.EM_USO &&
            x.FaseDedicada_id == 9)), Times.Once);
    }

    [Test(Description = "TMAQCONT10 - Delete deve devolver no content quando o pedido e valido.")]
    public async Task Delete_Should_ReturnNoContent_When_RequestIsValid()
    {
        // ARRANGE

        // ACT
        var result = await _controller.Delete(12);

        // ASSERT
        result.Should().BeOfType<NoContentResult>();
        _maquinaService.Verify(s => s.DeleteAsync(12), Times.Once);
    }
}
