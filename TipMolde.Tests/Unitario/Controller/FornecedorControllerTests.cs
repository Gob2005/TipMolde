using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TipMolde.API.Controllers;
using TipMolde.Application.DTOs.FornecedorDTO;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Comercio.IFornecedor;

namespace TipMolde.Tests.Unitario.Controller;

[TestFixture]
[Category("Unit")]
public class FornecedorControllerTests
{
    private Mock<IFornecedorService> _fornecedorService = null!;
    private Mock<ILogger<FornecedorController>> _logger = null!;
    private FornecedorController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        // ARRANGE
        _fornecedorService = new Mock<IFornecedorService>();
        _logger = new Mock<ILogger<FornecedorController>>();

        _controller = new FornecedorController(_fornecedorService.Object, _logger.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    private static ResponseFornecedorDTO BuildResponse(int id = 1, string nome = "Fornecedor A")
    {
        return new ResponseFornecedorDTO
        {
            FornecedorId = id,
            Nome = nome,
            NIF = "123456789",
            Morada = "Rua A",
            Email = "fornecedor@tipmolde.pt",
            Telefone = "910000000"
        };
    }

    [Test(Description = "T1FORCONT - GetAll deve devolver bad request quando paginacao e invalida.")]
    public async Task GetAll_Should_ReturnBadRequest_When_PaginationIsInvalid()
    {
        // ARRANGE

        // ACT
        var result = await _controller.GetAll(0, 10);

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
        _fornecedorService.Verify(s => s.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Test(Description = "T2FORCONT - GetById deve devolver not found quando fornecedor nao existe.")]
    public async Task GetById_Should_ReturnNotFound_When_FornecedorDoesNotExist()
    {
        // ARRANGE
        _fornecedorService.Setup(s => s.GetByIdAsync(88)).ReturnsAsync((ResponseFornecedorDTO?)null);

        // ACT
        var result = await _controller.GetById(88);

        // ASSERT
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test(Description = "T3FORCONT - GetById deve devolver dto quando fornecedor existe.")]
    public async Task GetById_Should_ReturnOkWithDto_When_FornecedorExists()
    {
        // ARRANGE
        var response = BuildResponse(id: 3);
        _fornecedorService.Setup(s => s.GetByIdAsync(3)).ReturnsAsync(response);

        // ACT
        var result = await _controller.GetById(3);

        // ASSERT
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(response);
    }

    [Test(Description = "T4FORCONT - Search deve devolver bad request quando searchTerm esta vazio.")]
    public async Task SearchByName_Should_ReturnBadRequest_When_SearchTermIsBlank()
    {
        // ARRANGE

        // ACT
        var result = await _controller.SearchByName("   ", 1, 10);

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
        _fornecedorService.Verify(s => s.SearchByNameAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Test(Description = "T5FORCONT - Search deve devolver bad request quando paginacao e invalida.")]
    public async Task SearchByName_Should_ReturnBadRequest_When_PaginationIsInvalid()
    {
        // ARRANGE

        // ACT
        var result = await _controller.SearchByName("Fornecedor", 0, 10);

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
        _fornecedorService.Verify(s => s.SearchByNameAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Test(Description = "T6FORCONT - Search deve devolver payload paginado quando pedido e valido.")]
    public async Task SearchByName_Should_ReturnPagedPayload_When_RequestIsValid()
    {
        // ARRANGE
        var items = new List<ResponseFornecedorDTO>
        {
            BuildResponse(id: 1),
            BuildResponse(id: 2, nome: "Fornecedor B")
        };

        _fornecedorService
            .Setup(s => s.SearchByNameAsync("Fornecedor", 1, 10))
            .ReturnsAsync(new PagedResult<ResponseFornecedorDTO>(items, 2, 1, 10));

        // ACT
        var result = await _controller.SearchByName("Fornecedor", 1, 10);

        // ASSERT
        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(new PagedResult<ResponseFornecedorDTO>(items, 2, 1, 10));
    }

    [Test(Description = "T7FORCONT - Create deve devolver bad request quando model state e invalido.")]
    public async Task Create_Should_ReturnBadRequest_When_ModelStateIsInvalid()
    {
        // ARRANGE
        _controller.ModelState.AddModelError("Nome", "Obrigatorio");
        var dto = new CreateFornecedorDTO
        {
            Nome = "Fornecedor A",
            NIF = "123456789"
        };

        // ACT
        var result = await _controller.Create(dto);

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
        _fornecedorService.Verify(s => s.CreateAsync(It.IsAny<CreateFornecedorDTO>()), Times.Never);
    }

    [Test(Description = "T8FORCONT - Create deve devolver created at action quando dados sao validos.")]
    public async Task Create_Should_ReturnCreatedAtAction_When_RequestIsValid()
    {
        // ARRANGE
        var dto = new CreateFornecedorDTO
        {
            Nome = "Fornecedor A",
            NIF = "123456789",
            Morada = "Rua A",
            Email = "fornecedor@tipmolde.pt",
            Telefone = "910000000"
        };

        var response = BuildResponse(id: 100);

        _fornecedorService.Setup(s => s.CreateAsync(dto)).ReturnsAsync(response);

        // ACT
        var result = await _controller.Create(dto);

        // ASSERT
        var created = result as CreatedAtActionResult;
        created.Should().NotBeNull();
        created!.ActionName.Should().Be(nameof(FornecedorController.GetById));
        created.RouteValues.Should().ContainKey("id");
        created.RouteValues!["id"].Should().Be(100);
        created.Value.Should().BeEquivalentTo(response);
    }

    [Test(Description = "T9FORCONT - Update deve devolver bad request quando model state e invalido.")]
    public async Task Update_Should_ReturnBadRequest_When_ModelStateIsInvalid()
    {
        // ARRANGE
        _controller.ModelState.AddModelError("Nome", "Invalido");
        var dto = new UpdateFornecedorDTO
        {
            Nome = "Fornecedor B"
        };

        // ACT
        var result = await _controller.Update(5, dto);

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
        _fornecedorService.Verify(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<UpdateFornecedorDTO>()), Times.Never);
    }

    [Test(Description = "T10FORCONT - Update deve devolver no content quando pedido e valido.")]
    public async Task Update_Should_ReturnNoContent_When_RequestIsValid()
    {
        // ARRANGE
        var dto = new UpdateFornecedorDTO
        {
            Nome = "Fornecedor Novo",
            Email = "novo@tipmolde.pt"
        };

        // ACT
        var result = await _controller.Update(5, dto);

        // ASSERT
        result.Should().BeOfType<NoContentResult>();
        _fornecedorService.Verify(s => s.UpdateAsync(5, It.Is<UpdateFornecedorDTO>(f =>
            f.Nome == "Fornecedor Novo" &&
            f.Email == "novo@tipmolde.pt")), Times.Once);
    }

    [Test(Description = "T11FORCONT - Delete deve devolver no content quando pedido e valido.")]
    public async Task Delete_Should_ReturnNoContent_When_RequestIsValid()
    {
        // ARRANGE

        // ACT
        var result = await _controller.Delete(12);

        // ASSERT
        result.Should().BeOfType<NoContentResult>();
        _fornecedorService.Verify(s => s.DeleteAsync(12), Times.Once);
    }
}
