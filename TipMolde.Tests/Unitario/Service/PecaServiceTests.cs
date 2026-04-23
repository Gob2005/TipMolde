using FluentAssertions;
using Moq;
using TipMolde.Application.Interface.Producao.IMolde;
using TipMolde.Application.Interface.Producao.IPeca;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;
using TipMolde.Application.Service;

namespace TipMolde.Tests.Unitario.Service;

[TestFixture]
[Category("Unit")]
public class PecaServiceTests
{
    private Mock<IPecaRepository> _pecaRepository = null!;
    private Mock<IMoldeRepository> _moldeRepository = null!;
    private PecaService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _pecaRepository = new Mock<IPecaRepository>();
        _moldeRepository = new Mock<IMoldeRepository>();
        _sut = new PecaService(_pecaRepository.Object, _moldeRepository.Object);
    }

    private static Molde BuildMolde(int id = 1) => new()
    {
        Molde_id = id,
        Numero = $"M-{id}",
        Numero_cavidades = 1,
        TipoPedido = TipoPedido.NOVO_MOLDE
    };

    private static Peca BuildPeca(int id = 1, int moldeId = 1, string designacao = "Extrator") => new()
    {
        Peca_id = id,
        Designacao = designacao,
        Prioridade = 1,
        MaterialDesignacao = "Aco",
        MaterialRecebido = false,
        Molde_id = moldeId
    };

    [Test]
    public async Task shouldCreatePecaWhenDataIsValid()
    {
        // Arrange
        var peca = BuildPeca();
        _moldeRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildMolde());
        _pecaRepository.Setup(r => r.GetByDesignacaoAsync("Extrator", 1)).ReturnsAsync((Peca?)null);

        // Act
        var result = await _sut.CreateAsync(peca);

        // Assert
        result.Designacao.Should().Be("Extrator");
        _pecaRepository.Verify(r => r.AddAsync(It.IsAny<Peca>()), Times.Once);
    }

    [Test]
    public async Task shouldThrowKeyNotFoundExceptionWhenMoldeDoesNotExist()
    {
        // Arrange
        _moldeRepository.Setup(r => r.GetByIdAsync(7)).ReturnsAsync((Molde?)null);

        // Act
        Func<Task> act = () => _sut.CreateAsync(BuildPeca(moldeId: 7));

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test]
    public async Task shouldThrowArgumentExceptionWhenDesignacaoIsBlank()
    {
        // Arrange
        _moldeRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildMolde());

        // Act
        Func<Task> act = () => _sut.CreateAsync(BuildPeca(designacao: " "));

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task shouldThrowArgumentExceptionWhenDesignacaoAlreadyExistsForMolde()
    {
        // Arrange
        _moldeRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildMolde());
        _pecaRepository.Setup(r => r.GetByDesignacaoAsync("Extrator", 1)).ReturnsAsync(BuildPeca(id: 2));

        // Act
        Func<Task> act = () => _sut.CreateAsync(BuildPeca());

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task shouldThrowKeyNotFoundExceptionWhenUpdatingUnknownPeca()
    {
        // Arrange
        _pecaRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Peca?)null);

        // Act
        Func<Task> act = () => _sut.UpdateAsync(BuildPeca(id: 99));

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test]
    public async Task shouldUpdatePecaWhenIncomingValuesAreValid()
    {
        // Arrange
        var existing = BuildPeca(id: 1, designacao: "A");
        _pecaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        var update = BuildPeca(id: 1, designacao: "  B  ");
        update.Prioridade = 5;
        update.MaterialDesignacao = "Inox";
        update.MaterialRecebido = true;

        // Act
        await _sut.UpdateAsync(update);

        // Assert
        _pecaRepository.Verify(r => r.UpdateAsync(It.Is<Peca>(p =>
            p.Peca_id == 1 &&
            p.Designacao == "B" &&
            p.Prioridade == 5 &&
            p.MaterialDesignacao == "Inox" &&
            p.MaterialRecebido)), Times.Once);
    }

    [Test]
    public async Task shouldKeepPreviousValuesWhenIncomingValuesAreInvalid()
    {
        // Arrange
        var existing = BuildPeca(id: 1, designacao: "Original");
        existing.Prioridade = 3;
        existing.MaterialDesignacao = "Aco";

        _pecaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        var update = BuildPeca(id: 1, designacao: " ");
        update.Prioridade = 0;
        update.MaterialDesignacao = null;

        // Act
        await _sut.UpdateAsync(update);

        // Assert
        _pecaRepository.Verify(r => r.UpdateAsync(It.Is<Peca>(p =>
            p.Designacao == "Original" &&
            p.Prioridade == 3 &&
            p.MaterialDesignacao == "Aco")), Times.Once);
    }

    [Test]
    public async Task shouldThrowKeyNotFoundExceptionWhenDeletingUnknownPeca()
    {
        // Arrange
        _pecaRepository.Setup(r => r.GetByIdAsync(10)).ReturnsAsync((Peca?)null);

        // Act
        Func<Task> act = () => _sut.DeleteAsync(10);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test]
    public async Task shouldDeletePecaWhenPecaExists()
    {
        // Arrange
        _pecaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildPeca());

        // Act
        await _sut.DeleteAsync(1);

        // Assert
        _pecaRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Test]
    public async Task shouldReturnPecaWhenSearchingByDesignacao()
    {
        // Arrange
        _pecaRepository.Setup(r => r.GetByDesignacaoAsync("Extrator", 2))
            .ReturnsAsync(BuildPeca(id: 5, moldeId: 2));

        // Act
        var result = await _sut.GetByDesignacaoAsync("Extrator", 2);

        // Assert
        result.Should().NotBeNull();
        result!.Peca_id.Should().Be(5);
    }
}
