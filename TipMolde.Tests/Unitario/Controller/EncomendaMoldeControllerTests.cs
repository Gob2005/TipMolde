using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TipMolde.API.Controllers;
using TipMolde.Application.DTOs.EncomendaMoldeDTO;
using TipMolde.Application.Interface.Comercio.IEncomendaMolde;

namespace TipMolde.Tests.Unitario.Controller;

[TestFixture]
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
        var dto = new CreateEncomendaMoldeDTO
        {
            Encomenda_id = 1,
            Molde_id = 2,
            Quantidade = 10,
            Prioridade = 1,
            DataEntregaPrevista = DateTime.UtcNow.AddDays(5)
        };

        var response = new ResponseEncomendaMoldeDTO
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
}
