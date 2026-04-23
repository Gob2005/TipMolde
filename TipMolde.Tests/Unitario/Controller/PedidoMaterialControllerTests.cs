using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TipMolde.API.Controllers;
using TipMolde.Application.DTOs.PedidoMaterialDTO;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Comercio.IPedidoMaterial;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Unitario.Controller;

[TestFixture]
[Category("Unit")]
public class PedidoMaterialControllerTests
{
    private Mock<IPedidoMaterialService> _service = null!;
    private Mock<ILogger<PedidoMaterialController>> _logger = null!;
    private PedidoMaterialController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        // ARRANGE
        _service = new Mock<IPedidoMaterialService>();
        _logger = new Mock<ILogger<PedidoMaterialController>>();

        _controller = new PedidoMaterialController(_service.Object, _logger.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    private static ResponsePedidoMaterialDTO BuildResponse(int id = 1) => new()
    {
        PedidoMaterialId = id,
        DataPedido = new DateTime(2026, 4, 23, 10, 0, 0, DateTimeKind.Utc),
        Estado = EstadoPedido.PENDENTE,
        FornecedorId = 10,
        Itens =
        {
            new ResponseItemPedidoMaterialDTO { ItemId = 1, PecaId = 100, Quantidade = 2 }
        }
    };

    [Test(Description = "TPMCONT1 - GetAll deve devolver bad request quando paginacao e invalida.")]
    public async Task GetAll_Should_ReturnBadRequest_When_PaginationIsInvalid()
    {
        // ARRANGE

        // ACT
        var result = await _controller.GetAll(0, 10);

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
        _service.Verify(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Test(Description = "TPMCONT2 - GetById deve devolver not found quando pedido nao existe.")]
    public async Task GetById_Should_ReturnNotFound_When_PedidoDoesNotExist()
    {
        // ARRANGE
        _service.Setup(s => s.GetByIdAsync(44)).ReturnsAsync((ResponsePedidoMaterialDTO?)null);

        // ACT
        var result = await _controller.GetById(44);

        // ASSERT
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test(Description = "TPMCONT3 - Create deve devolver created at action quando request e valida.")]
    public async Task Create_Should_ReturnCreatedAtAction_When_RequestIsValid()
    {
        // ARRANGE
        var dto = new CreatePedidoMaterialDTO
        {
            Fornecedor_id = 10,
            Itens =
            {
                new CreateItemPedidoMaterialDTO { Peca_id = 100, Quantidade = 3 }
            }
        };

        var response = BuildResponse(id: 99);
        _service.Setup(s => s.CreateAsync(dto)).ReturnsAsync(response);

        // ACT
        var result = await _controller.Create(dto);

        // ASSERT
        var created = result as CreatedAtActionResult;
        created.Should().NotBeNull();
        created!.ActionName.Should().Be(nameof(PedidoMaterialController.GetById));
        created.RouteValues!["id"].Should().Be(99);
        created.Value.Should().BeEquivalentTo(response);
    }

    [Test(Description = "TPMCONT4 - RegistarRececao deve usar o user id do token e devolver no content.")]
    public async Task RegistarRececao_Should_UseAuthenticatedUserId_When_TokenIsValid()
    {
        // ARRANGE
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "7")
        }, "TestAuth");

        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

        // ACT
        var result = await _controller.RegistarRececao(25);

        // ASSERT
        result.Should().BeOfType<NoContentResult>();
        _service.Verify(s => s.RegistarRececaoAsync(25, 7), Times.Once);
    }

    [Test(Description = "TPMCONT5 - RegistarRececao deve falhar quando token nao contem utilizador valido.")]
    public void RegistarRececao_Should_ThrowUnauthorizedAccessException_When_TokenIsInvalid()
    {
        // ARRANGE
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "abc")
        }, "TestAuth");

        _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(identity);

        // ACT
        Func<Task> act = () => _controller.RegistarRececao(25);

        // ASSERT
        act.Should().ThrowAsync<UnauthorizedAccessException>();
    }
}
