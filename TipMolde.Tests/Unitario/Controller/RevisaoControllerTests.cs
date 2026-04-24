using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TipMolde.API.Controllers;
using TipMolde.Application.Dtos.RevisaoDto;
using TipMolde.Application.Interface.Desenho.IRevisao;

namespace TipMolde.Tests.Unitario.Controller;

[TestFixture]
[Category("Unit")]
public class RevisaoControllerTests
{
    private Mock<IRevisaoService> _revisaoService = null!;
    private Mock<ILogger<RevisaoController>> _logger = null!;
    private RevisaoController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _revisaoService = new Mock<IRevisaoService>();
        _logger = new Mock<ILogger<RevisaoController>>();

        _controller = new RevisaoController(
            _revisaoService.Object,
            _logger.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    private static ResponseRevisaoDto BuildResponse(int id = 1)
    {
        return new ResponseRevisaoDto
        {
            Revisao_id = id,
            NumRevisao = 2,
            DescricaoAlteracoes = "Rev",
            DataEnvioCliente = DateTime.UtcNow,
            Projeto_id = 7
        };
    }

    [Test(Description = "TREVCONT1 - GetByProjeto deve devolver bad request quando projetoId e invalido.")]
    public async Task GetByProjeto_Should_ReturnBadRequest_When_ProjetoIdIsInvalid()
    {
        // ACT
        var result = await _controller.GetByProjeto(0);

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test(Description = "TREVCONT2 - GetById deve devolver not found quando a revisao nao existe.")]
    public async Task GetById_Should_ReturnNotFound_When_RevisaoDoesNotExist()
    {
        // ARRANGE
        _revisaoService.Setup(s => s.GetByIdAsync(50)).ReturnsAsync((ResponseRevisaoDto?)null);

        // ACT
        var result = await _controller.GetById(50);

        // ASSERT
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test(Description = "TREVCONT3 - Create deve devolver status 400 quando model state e invalido.")]
    public async Task Create_Should_ReturnStatus400_When_ModelStateIsInvalid()
    {
        // ARRANGE
        _controller.ModelState.AddModelError("DescricaoAlteracoes", "Obrigatorio");

        var dto = new CreateRevisaoDto
        {
            Projeto_id = 3,
            DescricaoAlteracoes = "Nova revisao"
        };

        // ACT
        var result = await _controller.Create(dto);

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
        _revisaoService.Verify(s => s.CreateAsync(It.IsAny<CreateRevisaoDto>()), Times.Never);
    }

    [Test(Description = "TREVCONT4 - Create deve devolver created at action quando pedido e valido.")]
    public async Task Create_Should_ReturnCreatedAtAction_When_RequestIsValid()
    {
        // ARRANGE
        var dto = new CreateRevisaoDto
        {
            Projeto_id = 3,
            DescricaoAlteracoes = "Nova revisao"
        };

        var response = BuildResponse(id: 22);

        _revisaoService.Setup(s => s.CreateAsync(dto)).ReturnsAsync(response);

        // ACT
        var result = await _controller.Create(dto);

        // ASSERT
        var created = result as CreatedAtActionResult;
        created.Should().NotBeNull();
        created!.ActionName.Should().Be(nameof(RevisaoController.GetById));
        created.RouteValues!["id"].Should().Be(22);
        created.Value.Should().BeEquivalentTo(response);
    }

    [Test(Description = "TREVCONT5 - UpdateRespostaCliente deve devolver bad request quando model state e invalido.")]
    public async Task UpdateRespostaCliente_Should_ReturnBadRequest_When_ModelStateIsInvalid()
    {
        // ARRANGE
        _controller.ModelState.AddModelError("Aprovado", "Obrigatorio");

        // ACT
        var result = await _controller.UpdateRespostaCliente(9, new UpdateRespostaRevisaoDto());

        // ASSERT
        result.Should().BeOfType<BadRequestObjectResult>();
        _revisaoService.Verify(s => s.UpdateRespostaClienteAsync(It.IsAny<int>(), It.IsAny<UpdateRespostaRevisaoDto>()), Times.Never);
    }

    [Test(Description = "TREVCONT6 - UpdateRespostaCliente deve devolver no content quando pedido e valido.")]
    public async Task UpdateRespostaCliente_Should_ReturnNoContent_When_RequestIsValid()
    {
        // ARRANGE
        var dto = new UpdateRespostaRevisaoDto
        {
            Aprovado = true
        };

        // ACT
        var result = await _controller.UpdateRespostaCliente(9, dto);

        // ASSERT
        result.Should().BeOfType<NoContentResult>();
        _revisaoService.Verify(s => s.UpdateRespostaClienteAsync(9, dto), Times.Once);
    }
}
