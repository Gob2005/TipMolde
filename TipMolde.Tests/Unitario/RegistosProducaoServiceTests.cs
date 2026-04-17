using FluentAssertions;
using Moq;
using TipMolde.Application.Interface.Producao.IFasesProducao;
using TipMolde.Application.Interface.Producao.IMaquina;
using TipMolde.Application.Interface.Producao.IPeca;
using TipMolde.Application.Interface.Producao.IRegistosProducao;
using TipMolde.Application.Interface.Utilizador.IUser;
using TipMolde.Domain.Entities;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;
using TipMolde.Application.Service;

namespace TipMolde.Tests.Unitario;

[TestFixture]
public class RegistosProducaoServiceTests
{
    private Mock<IRegistosProducaoRepository> _registosRepository = null!;
    private Mock<IFasesProducaoRepository> _fasesRepository = null!;
    private Mock<IUserRepository> _userRepository = null!;
    private Mock<IMaquinaRepository> _maquinaRepository = null!;
    private Mock<IPecaRepository> _pecaRepository = null!;
    private RegistosProducaoService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _registosRepository = new Mock<IRegistosProducaoRepository>();
        _fasesRepository = new Mock<IFasesProducaoRepository>();
        _userRepository = new Mock<IUserRepository>();
        _maquinaRepository = new Mock<IMaquinaRepository>();
        _pecaRepository = new Mock<IPecaRepository>();

        _sut = new RegistosProducaoService(
            _registosRepository.Object,
            _fasesRepository.Object,
            _userRepository.Object,
            _maquinaRepository.Object,
            _pecaRepository.Object);
    }

    private static FasesProducao BuildFase(int id = 1) => new()
    {
        Fases_producao_id = id,
        Nome = Nome_fases.MAQUINACAO,
        Descricao = "Fase teste"
    };

    private static User BuildOperador(int id = 1) => new()
    {
        User_id = id,
        Nome = "Operador",
        Email = "op@tipmolde.pt",
        Password = "Hash123!",
        Role = UserRole.GESTOR_PRODUCAO
    };

    private static Peca BuildPeca(int id = 1, bool materialRecebido = true) => new()
    {
        Peca_id = id,
        Designacao = "Peca",
        Prioridade = 1,
        MaterialDesignacao = "Aco",
        MaterialRecebido = materialRecebido,
        Molde_id = 1
    };

    private static Maquina BuildMaquina(int id = 1, int faseId = 1, EstadoMaquina estado = EstadoMaquina.DISPONIVEL) => new()
    {
        Maquina_id = id,
        Numero = id,
        NomeModelo = "CNC",
        FaseDedicada_id = faseId,
        Estado = estado
    };

    private static RegistosProducao BuildRegisto(int faseId = 1, int operadorId = 1, int pecaId = 1, EstadoProducao estado = EstadoProducao.PREPARACAO, int? maquinaId = 1) => new()
    {
        Fase_id = faseId,
        Operador_id = operadorId,
        Peca_id = pecaId,
        Estado_producao = estado,
        Maquina_id = maquinaId
    };

    [Test]
    public async Task shouldThrowKeyNotFoundExceptionWhenFaseDoesNotExist()
    {
        // Arrange
        _fasesRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((FasesProducao?)null);

        // Act
        Func<Task> act = () => _sut.CreateAsync(BuildRegisto(faseId: 99));

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test]
    public async Task shouldThrowKeyNotFoundExceptionWhenOperadorDoesNotExist()
    {
        // Arrange
        _fasesRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildFase());
        _userRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        // Act
        Func<Task> act = () => _sut.CreateAsync(BuildRegisto(operadorId: 99));

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test]
    public async Task shouldThrowKeyNotFoundExceptionWhenPecaDoesNotExist()
    {
        // Arrange
        _fasesRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildFase());
        _userRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildOperador());
        _pecaRepository.Setup(r => r.GetByIdAsync(88)).ReturnsAsync((Peca?)null);

        // Act
        Func<Task> act = () => _sut.CreateAsync(BuildRegisto(pecaId: 88));

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test]
    public async Task shouldThrowArgumentExceptionWhenMaterialWasNotReceived()
    {
        // Arrange
        _fasesRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildFase());
        _userRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildOperador());
        _pecaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildPeca(materialRecebido: false));

        // Act
        Func<Task> act = () => _sut.CreateAsync(BuildRegisto());

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task shouldThrowArgumentExceptionWhenFirstTransitionIsNotPreparacao()
    {
        // Arrange
        _fasesRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildFase());
        _userRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildOperador());
        _pecaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildPeca());
        _registosRepository.Setup(r => r.GetUltimoRegistoAsync(1, 1)).ReturnsAsync((RegistosProducao?)null);

        // Act
        Func<Task> act = () => _sut.CreateAsync(BuildRegisto(estado: EstadoProducao.EM_CURSO));

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task shouldCreateRegistoAndSetMachineInUseWhenTransitionIsPreparacao()
    {
        // Arrange
        _fasesRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildFase());
        _userRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildOperador());
        _pecaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildPeca());
        _registosRepository.Setup(r => r.GetUltimoRegistoAsync(1, 1)).ReturnsAsync((RegistosProducao?)null);

        var maquina = BuildMaquina(id: 1, faseId: 1, estado: EstadoMaquina.DISPONIVEL);
        _maquinaRepository.Setup(r => r.GetByIdUnicoAsync(1)).ReturnsAsync(maquina);

        var registo = BuildRegisto(estado: EstadoProducao.PREPARACAO, maquinaId: 1);

        // Act
        var result = await _sut.CreateAsync(registo);

        // Assert
        result.Estado_producao.Should().Be(EstadoProducao.PREPARACAO);
        result.Data_hora.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        maquina.Estado.Should().Be(EstadoMaquina.EM_USO);
        _maquinaRepository.Verify(r => r.UpdateAsync(It.Is<Maquina>(m => m.Estado == EstadoMaquina.EM_USO)), Times.Once);
        _registosRepository.Verify(r => r.AddAsync(It.IsAny<RegistosProducao>()), Times.Once);
    }

    [Test]
    public async Task shouldThrowArgumentExceptionWhenMachinePhaseDoesNotMatchRegistoPhase()
    {
        // Arrange
        _fasesRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildFase());
        _userRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildOperador());
        _pecaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildPeca());
        _registosRepository.Setup(r => r.GetUltimoRegistoAsync(1, 1)).ReturnsAsync((RegistosProducao?)null);
        _maquinaRepository.Setup(r => r.GetByIdUnicoAsync(1)).ReturnsAsync(BuildMaquina(id: 1, faseId: 2));

        // Act
        Func<Task> act = () => _sut.CreateAsync(BuildRegisto(estado: EstadoProducao.PREPARACAO, maquinaId: 1));

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task shouldThrowArgumentExceptionWhenMachineIsUnavailable()
    {
        // Arrange
        _fasesRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildFase());
        _userRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildOperador());
        _pecaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildPeca());
        _registosRepository.Setup(r => r.GetUltimoRegistoAsync(1, 1)).ReturnsAsync((RegistosProducao?)null);
        _maquinaRepository.Setup(r => r.GetByIdUnicoAsync(1)).ReturnsAsync(BuildMaquina(id: 1, faseId: 1, estado: EstadoMaquina.EM_USO));

        // Act
        Func<Task> act = () => _sut.CreateAsync(BuildRegisto(estado: EstadoProducao.PREPARACAO, maquinaId: 1));

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task shouldReleaseMachineWhenTransitionIsConcluido()
    {
        // Arrange
        _fasesRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildFase());
        _userRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildOperador());
        _pecaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildPeca());
        _registosRepository.Setup(r => r.GetUltimoRegistoAsync(1, 1)).ReturnsAsync(new RegistosProducao
        {
            Fase_id = 1,
            Peca_id = 1,
            Estado_producao = EstadoProducao.EM_CURSO,
            Data_hora = DateTime.UtcNow.AddMinutes(-5)
        });

        var maquina = BuildMaquina(id: 1, faseId: 1, estado: EstadoMaquina.EM_USO);
        _maquinaRepository.Setup(r => r.GetByIdUnicoAsync(1)).ReturnsAsync(maquina);

        // Act
        await _sut.CreateAsync(BuildRegisto(estado: EstadoProducao.CONCLUIDO, maquinaId: 1));

        // Assert
        maquina.Estado.Should().Be(EstadoMaquina.DISPONIVEL);
        _maquinaRepository.Verify(r => r.UpdateAsync(It.Is<Maquina>(m => m.Estado == EstadoMaquina.DISPONIVEL)), Times.Once);
    }

    [Test]
    public async Task shouldThrowArgumentExceptionWhenTransitionIsInvalid()
    {
        // Arrange
        _fasesRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildFase());
        _userRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildOperador());
        _pecaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(BuildPeca());
        _registosRepository.Setup(r => r.GetUltimoRegistoAsync(1, 1)).ReturnsAsync(new RegistosProducao
        {
            Fase_id = 1,
            Peca_id = 1,
            Estado_producao = EstadoProducao.PREPARACAO,
            Data_hora = DateTime.UtcNow.AddMinutes(-5)
        });

        // Act
        Func<Task> act = () => _sut.CreateAsync(BuildRegisto(estado: EstadoProducao.CONCLUIDO));

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}
