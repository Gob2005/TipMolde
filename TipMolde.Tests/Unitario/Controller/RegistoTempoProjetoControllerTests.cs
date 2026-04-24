using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TipMolde.API.Controllers;
using TipMolde.Application.Dtos.RegistoTempoProjetoDto;
using TipMolde.Application.Interface.Desenho.IRegistoTempoProjeto;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Unitario.Controller;

[TestFixture]
[Category("Unit")]
public class RegistoTempoProjetoControllerTests
{
    private Mock<IRegistoTempoProjetoService> _service = null!;
    private Mock<ILogger<RegistoTempoProjetoController>> _logger = null!;
    private RegistoTempoProjetoController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _service = new Mock<IRegistoTempoProjetoService>();
        _logger = new Mock<ILogger<RegistoTempoProjetoController>>();

        _controller = new RegistoTempoProjetoController(_service.Object, _logger.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    private static ResponseRegistoTempoProjetoDto BuildResponse(int id = 1)
    {
        return new ResponseRegistoTempoProjetoDto
        {
            Registo_Tempo_Projeto_id = id,
            Estado_tempo = EstadoTempoProjeto.INICIADO,
            Data_hora = new DateTime(2026, 4, 24, 9, 0, 0, DateTimeKind.Utc),
            Projeto_id = 10,
            Autor_id = 5,
            Peca_id = 3
        };
    }

    [Test(Description = "TRTPCONT1 - GetHistorico deve devolver bad request quando os ids de query sao invalidos.")]
    public async Task GetHistorico_Should_ReturnBadRequest_When_QueryIdsAreInvalid()
    {
        // ACT
        var result = await _controller.GetHistorico(0, 5);

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test(Description = "TRTPCONT2 - GetById deve devolver not found quando o registo nao existe.")]
    public async Task GetById_Should_ReturnNotFound_When_RegistoDoesNotExist()
    {
        // ARRANGE
        _service.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((ResponseRegistoTempoProjetoDto?)null);

        // ACT
        var result = await _controller.GetById(99);

        // ASSERT
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test(Description = "TRTPCONT3 - Create deve devolver status 400 quando o model state e invalido.")]
    public async Task Create_Should_ReturnBadRequest_When_ModelStateIsInvalid()
    {
        // ARRANGE
        _controller.ModelState.AddModelError("Estado_tempo", "Obrigatorio");

        var dto = new CreateRegistoTempoProjetoDto
        {
            Estado_tempo = EstadoTempoProjeto.INICIADO,
            Projeto_id = 10,
            Autor_id = 5,
            Peca_id = 3
        };

        // ACT
        var result = await _controller.Create(dto);

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
        _service.Verify(s => s.CreateRegistoAsync(It.IsAny<CreateRegistoTempoProjetoDto>()), Times.Never);
    }

    [Test(Description = "TRTPCONT4 - Create deve devolver created at action quando o pedido e valido.")]
    public async Task Create_Should_ReturnCreatedAtAction_When_RequestIsValid()
    {
        // ARRANGE
        var dto = new CreateRegistoTempoProjetoDto
        {
            Estado_tempo = EstadoTempoProjeto.INICIADO,
            Projeto_id = 10,
            Autor_id = 5,
            Peca_id = 3
        };

        var response = BuildResponse(25);
        _service.Setup(s => s.CreateRegistoAsync(dto)).ReturnsAsync(response);

        // ACT
        var result = await _controller.Create(dto);

        // ASSERT
        var created = result as CreatedAtActionResult;
        created.Should().NotBeNull();
        created!.ActionName.Should().Be(nameof(RegistoTempoProjetoController.GetById));
        created.RouteValues!["id"].Should().Be(25);
        created.Value.Should().BeEquivalentTo(response);
    }

    [Test(Description = "TRTPCONT5 - Delete deve devolver no content quando o pedido e valido.")]
    public async Task Delete_Should_ReturnNoContent_When_RequestIsValid()
    {
        // ACT
        var result = await _controller.Delete(15);

        // ASSERT
        result.Should().BeOfType<NoContentResult>();
        _service.Verify(s => s.DeleteAsync(15), Times.Once);
    }
}
