using FluentAssertions;
using Moq;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Producao.IFasesProducao;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;
using TipMolde.Application.Service;

namespace TipMolde.Tests.Unitario.Service;

[TestFixture]
[Category("Unit")]
public class FasesProducaoServiceTests
{
    private Mock<IFasesProducaoRepository> _repository = null!;
    private FasesProducaoService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<IFasesProducaoRepository>();
        _sut = new FasesProducaoService(_repository.Object);
    }

    private static FasesProducao BuildFase(
        int id = 1,
        Nome_fases nome = Nome_fases.MAQUINACAO,
        string descricao = "Fase de maquina��o") => new()
        {
            Fases_producao_id = id,
            Nome = nome,
            Descricao = descricao
        };

    [Test]
    public async Task shouldCreateFaseWhenNomeIsUnique()
    {
        // Arrange
        var fase = BuildFase();
        _repository.Setup(r => r.GetByNomeAsync(Nome_fases.MAQUINACAO)).ReturnsAsync((FasesProducao?)null);

        // Act
        var result = await _sut.CreateAsync(fase);

        // Assert
        result.Nome.Should().Be(Nome_fases.MAQUINACAO);
        _repository.Verify(r => r.AddAsync(It.IsAny<FasesProducao>()), Times.Once);
    }

    [Test]
    public async Task shouldThrowArgumentExceptionWhenNomeAlreadyExists()
    {
        // Arrange
        _repository.Setup(r => r.GetByNomeAsync(Nome_fases.MAQUINACAO)).ReturnsAsync(BuildFase());

        // Act
        Func<Task> act = () => _sut.CreateAsync(BuildFase(id: 0));

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [TestCase(Nome_fases.MAQUINACAO)]
    [TestCase(Nome_fases.EROSAO)]
    [TestCase(Nome_fases.MONTAGEM)]
    public async Task shouldCreateFaseForEveryValidNome(Nome_fases nome)
    {
        // Arrange
        _repository.Setup(r => r.GetByNomeAsync(nome)).ReturnsAsync((FasesProducao?)null);

        // Act
        var result = await _sut.CreateAsync(new FasesProducao { Nome = nome, Descricao = "Descri��o de teste" });

        // Assert
        result.Nome.Should().Be(nome);
    }

    [Test]
    public async Task shouldUpdateFaseWhenDataIsValid()
    {
        // Arrange
        var existing = BuildFase(id: 1, nome: Nome_fases.MAQUINACAO, descricao: "Antiga");
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _repository.Setup(r => r.GetByNomeAsync(Nome_fases.EROSAO)).ReturnsAsync((FasesProducao?)null);

        var update = new FasesProducao
        {
            Fases_producao_id = 1,
            Nome = Nome_fases.EROSAO,
            Descricao = "Descri��o atualizada"
        };

        // Act
        await _sut.UpdateAsync(update);

        // Assert
        _repository.Verify(r => r.UpdateAsync(It.Is<FasesProducao>(f =>
            f.Nome == Nome_fases.EROSAO &&
            f.Descricao == "Descri��o atualizada")), Times.Once);
    }

    [Test]
    public async Task shouldThrowKeyNotFoundExceptionWhenUpdatingUnknownFase()
    {
        // Arrange
        _repository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((FasesProducao?)null);

        // Act
        Func<Task> act = () => _sut.UpdateAsync(BuildFase(id: 999));

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test]
    public async Task shouldThrowArgumentExceptionWhenUpdatingToNomeAlreadyUsedByOtherFase()
    {
        // Arrange
        var existing = BuildFase(id: 1, nome: Nome_fases.MAQUINACAO);
        var other = BuildFase(id: 2, nome: Nome_fases.EROSAO);
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
        _repository.Setup(r => r.GetByNomeAsync(Nome_fases.EROSAO)).ReturnsAsync(other);

        var update = new FasesProducao
        {
            Fases_producao_id = 1,
            Nome = Nome_fases.EROSAO,
            Descricao = "Sem conflito na descri��o"
        };

        // Act
        Func<Task> act = () => _sut.UpdateAsync(update);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task shouldUpdateDescricaoOnlyWhenNomeIsUnchanged()
    {
        // Arrange
        var existing = BuildFase(id: 1, nome: Nome_fases.MAQUINACAO, descricao: "Antiga");
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

        var update = new FasesProducao
        {
            Fases_producao_id = 1,
            Nome = Nome_fases.MAQUINACAO,
            Descricao = "Descri��o nova"
        };

        // Act
        await _sut.UpdateAsync(update);

        // Assert
        _repository.Verify(r => r.UpdateAsync(It.Is<FasesProducao>(f =>
            f.Nome == Nome_fases.MAQUINACAO &&
            f.Descricao == "Descri��o nova")), Times.Once);
    }

    [Test]
    public async Task shouldDeleteFaseWhenIdExists()
    {
        // Arrange
        _repository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildFase());

        // Act
        await _sut.DeleteAsync(1);

        // Assert
        _repository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Test]
    public async Task shouldThrowKeyNotFoundExceptionWhenDeletingUnknownFase()
    {
        // Arrange
        _repository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((FasesProducao?)null);

        // Act
        Func<Task> act = () => _sut.DeleteAsync(999);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test]
    public async Task shouldReturnPagedFasesFromRepository()
    {
        // Arrange
        var fases = new List<FasesProducao>
        {
            BuildFase(1, Nome_fases.MAQUINACAO),
            BuildFase(2, Nome_fases.EROSAO),
            BuildFase(3, Nome_fases.MONTAGEM)
        };

        _repository.Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new PagedResult<FasesProducao>(fases, fases.Count, 1, fases.Count));

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Items.Should().HaveCount(3);
    }
}
