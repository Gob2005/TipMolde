namespace TipMolde.Tests.Unitario.Service;

/// <summary>
/// Testes unitarios dos casos de uso da feature FichaProducao.
/// </summary>
/*[TestFixture]
[Category("Unit")]
public class FichaProducaoServiceTests
{
    private Mock<IFichaProducaoRepository> _fichaRepository = null!;
    private Mock<IEncomendaMoldeRepository> _encomendaMoldeRepository = null!;
    private Mock<IUserRepository> _userRepository = null!;
    private Mock<IMaquinaRepository> _maquinaRepository = null!;
    private IMapper _mapper = null!;
    private FichaProducaoService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _fichaRepository = new Mock<IFichaProducaoRepository>();
        _encomendaMoldeRepository = new Mock<IEncomendaMoldeRepository>();
        _userRepository = new Mock<IUserRepository>();
        _maquinaRepository = new Mock<IMaquinaRepository>();

        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<FichaProducaoProfile>());
        _mapper = mapperConfig.CreateMapper();

        _sut = new FichaProducaoService(
            _fichaRepository.Object,
            _encomendaMoldeRepository.Object,
            _userRepository.Object,
            _maquinaRepository.Object,
            _mapper);
    }

    [Test(Description = "TFP001 - Criacao falha quando a relacao EncomendaMolde nao existe.")]
    public async Task CreateAsync_Should_Throw_When_EncomendaMoldeDoesNotExist()
    {
        // ARRANGE
        _encomendaMoldeRepository
            .Setup(r => r.GetByIdAsync(7))
            .ReturnsAsync((EncomendaMolde?)null);

        var dto = new CreateFichaProducaoDto
        {
            Tipo = TipoFicha.FRE,
            EncomendaMolde_id = 7
        };

        // ACT
        Func<Task> act = () => _sut.CreateAsync(dto);

        // ASSERT
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test(Description = "TFP002 - Criacao devolve a ficha em rascunho quando o pedido e valido.")]
    public async Task CreateAsync_Should_ReturnCreatedFicha_When_RequestIsValid()
    {
        // ARRANGE
        _encomendaMoldeRepository
            .Setup(r => r.GetByIdAsync(7))
            .ReturnsAsync(BuildEncomendaMolde(7));

        _fichaRepository
            .Setup(r => r.AddAsync(It.IsAny<FichaProducao>()))
            .ReturnsAsync((FichaProducao ficha) =>
            {
                ficha.FichaProducao_id = 10;
                return ficha;
            });

        var dto = new CreateFichaProducaoDto
        {
            Tipo = TipoFicha.FRE,
            EncomendaMolde_id = 7
        };

        // ACT
        var result = await _sut.CreateAsync(dto);

        // ASSERT
        result.FichaProducao_id.Should().Be(10);
        result.Tipo.Should().Be(TipoFicha.FRE);
        result.Estado.Should().Be(EstadoFichaProducao.RASCUNHO);
        result.Ativa.Should().BeTrue();
        result.EncomendaMolde_id.Should().Be(7);
        _fichaRepository.Verify(r => r.AddAsync(It.Is<FichaProducao>(f =>
            f.Tipo == TipoFicha.FRE &&
            f.EncomendaMolde_id == 7 &&
            f.Estado == EstadoFichaProducao.RASCUNHO &&
            f.Ativa)), Times.Once);
    }

    [Test(Description = "TFP003 - Submissao falha quando uma ficha FRE nao tem registos de ensaio.")]
    public async Task SubmitAsync_Should_Throw_When_FreHasNoRegistos()
    {
        // ARRANGE
        _fichaRepository
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(BuildFicha(id: 1, tipo: TipoFicha.FRE));
        _userRepository
            .Setup(r => r.GetByIdAsync(2))
            .ReturnsAsync(BuildUser(2));
        _fichaRepository
            .Setup(r => r.GetRegistosEnsaioByFichaIdAsync(1, 1, 1))
            .ReturnsAsync(new PagedResult<RegistoEnsaioFicha>(Array.Empty<RegistoEnsaioFicha>(), 0, 1, 1));

        // ACT
        Func<Task> act = () => _sut.SubmitAsync(1, 2);

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>();
        _fichaRepository.Verify(r => r.UpdateAsync(It.IsAny<FichaProducao>()), Times.Never);
    }

    [Test(Description = "TFP004 - Submissao marca a ficha como submetida quando o pedido e valido.")]
    public async Task SubmitAsync_Should_MarkFichaAsSubmitted_When_RequestIsValid()
    {
        // ARRANGE
        var ficha = BuildFicha(id: 2, tipo: TipoFicha.FLT);

        _fichaRepository
            .Setup(r => r.GetByIdAsync(2))
            .ReturnsAsync(ficha);
        _userRepository
            .Setup(r => r.GetByIdAsync(3))
            .ReturnsAsync(BuildUser(3));
        _fichaRepository
            .Setup(r => r.UpdateAsync(It.IsAny<FichaProducao>()))
            .Returns(Task.CompletedTask);

        // ACT
        var result = await _sut.SubmitAsync(2, 3);

        // ASSERT
        result.Estado.Should().Be(EstadoFichaProducao.SUBMETIDA);
        result.SubmetidaEm.Should().NotBeNull();
        ficha.SubmetidaPor_user_id.Should().Be(3);
        _fichaRepository.Verify(r => r.UpdateAsync(It.Is<FichaProducao>(f =>
            f.FichaProducao_id == 2 &&
            f.Estado == EstadoFichaProducao.SUBMETIDA &&
            f.SubmetidaPor_user_id == 3 &&
            f.SubmetidaEm.HasValue)), Times.Once);
    }

    [Test(Description = "TFP005 - Criacao de registo de ensaio falha quando a maquina nao existe.")]
    public async Task CreateRegistoEnsaioAsync_Should_Throw_When_MaquinaDoesNotExist()
    {
        // ARRANGE
        _userRepository
            .Setup(r => r.GetByIdAsync(4))
            .ReturnsAsync(BuildUser(4));
        _maquinaRepository
            .Setup(r => r.GetByIdAsync(9))
            .ReturnsAsync((Maquina?)null);

        var dto = new CreateRegistoEnsaioDto
        {
            LocalEnsaio = "Bancada A",
            AguasCavidade = true,
            AguasMacho = true,
            AguasMovimentos = false,
            ResumoTexto = "Resumo",
            Maquina_id = 9,
            Responsavel_id = 4
        };

        // ACT
        Func<Task> act = () => _sut.CreateRegistoEnsaioAsync(1, dto);

        // ASSERT
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test(Description = "TFP006 - Submissao de registo de ensaio torna o registo imutavel.")]
    public async Task SubmitRegistoEnsaioAsync_Should_MarkRegistoAsSubmitted_When_RequestIsValid()
    {
        // ARRANGE
        var ficha = BuildFicha(id: 5, tipo: TipoFicha.FRE);
        var registo = BuildRegistoEnsaio(registoId: 8, fichaId: 5);

        _userRepository
            .Setup(r => r.GetByIdAsync(6))
            .ReturnsAsync(BuildUser(6));
        _fichaRepository
            .Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(ficha);
        _fichaRepository
            .Setup(r => r.GetRegistoEnsaioByIdAsync(5, 8))
            .ReturnsAsync(registo);
        _fichaRepository
            .Setup(r => r.UpdateRegistoEnsaioAsync(It.IsAny<RegistoEnsaioFicha>()))
            .Returns(Task.CompletedTask);

        // ACT
        var result = await _sut.SubmitRegistoEnsaioAsync(5, 8, 6);

        // ASSERT
        result.Submetido.Should().BeTrue();
        result.SubmetidoPor_user_id.Should().Be(6);
        registo.SubmetidoEm.Should().NotBeNull();
        _fichaRepository.Verify(r => r.UpdateRegistoEnsaioAsync(It.Is<RegistoEnsaioFicha>(x =>
            x.RegistoEnsaioFicha_id == 8 &&
            x.Submetido &&
            x.SubmetidoPor_user_id == 6 &&
            x.SubmetidoEm.HasValue)), Times.Once);
    }

    [Test(Description = "TFP007 - Delete logico cancela a ficha em vez de apagar fisicamente.")]
    public async Task DeleteAsync_Should_CancelFicha_When_RequestIsValid()
    {
        // ARRANGE
        var ficha = BuildFicha(id: 9, tipo: TipoFicha.FRM);

        _fichaRepository
            .Setup(r => r.GetByIdAsync(9))
            .ReturnsAsync(ficha);
        _userRepository
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(BuildUser(1));
        _fichaRepository
            .Setup(r => r.UpdateAsync(It.IsAny<FichaProducao>()))
            .Returns(Task.CompletedTask);

        // ACT
        await _sut.DeleteAsync(9, 1);

        // ASSERT
        ficha.Ativa.Should().BeFalse();
        ficha.Estado.Should().Be(EstadoFichaProducao.CANCELADA);
        ficha.DesativadaPor_user_id.Should().Be(1);
        _fichaRepository.Verify(r => r.UpdateAsync(It.Is<FichaProducao>(f =>
            f.FichaProducao_id == 9 &&
            !f.Ativa &&
            f.Estado == EstadoFichaProducao.CANCELADA &&
            f.DesativadaPor_user_id == 1 &&
            f.DesativadaEm.HasValue)), Times.Once);
    }

    private static EncomendaMolde BuildEncomendaMolde(int id) => new()
    {
        EncomendaMolde_id = id,
        Encomenda_id = 1,
        Molde_id = 1,
        Quantidade = 1,
        Prioridade = 1,
        DataEntregaPrevista = DateTime.UtcNow.Date.AddDays(5)
    };

    private static FichaProducao BuildFicha(
        int id,
        TipoFicha tipo,
        EstadoFichaProducao estado = EstadoFichaProducao.RASCUNHO,
        bool ativa = true) => new()
    {
        FichaProducao_id = id,
        Tipo = tipo,
        Estado = estado,
        Ativa = ativa,
        DataCriacao = DateTime.UtcNow.AddDays(-1),
        EncomendaMolde_id = 1
    };

    private static User BuildUser(int id) => new()
    {
        User_id = id,
        Nome = "Utilizador Teste",
        Email = $"user{id}@tipmolde.pt",
        Password = "Hash123!",
        Role = UserRole.GESTOR_PRODUCAO
    };

    private static RegistoEnsaioFicha BuildRegistoEnsaio(int registoId, int fichaId) => new()
    {
        RegistoEnsaioFicha_id = registoId,
        FichaProducao_id = fichaId,
        LocalEnsaio = "Bancada A",
        AguasCavidade = true,
        AguasMacho = false,
        AguasMovimentos = true,
        ResumoTexto = "Resumo",
        Maquina_id = 2,
        Responsavel_id = 4,
        Submetido = false,
        CriadoEm = DateTime.UtcNow.AddMinutes(-10)
    };
}*/
