using Moq;
using TipMolde.Core.Enums;
using TipMolde.Core.Interface.IFases_producao;
using TipMolde.Core.Interface.IMolde;
using TipMolde.Core.Interface.IPeca;
using TipMolde.Core.Interface.IRegistosProducao;
using TipMolde.Core.Interface.IUser;
using TipMolde.Core.Models;
using TipMolde.Infrastructure.Service;

namespace TipMolde.Tests.Unitario
{
    /*public class RegistosProducaoServiceTests
    {
        private readonly Mock<IRegistosProducaoRepository> _rpRepository;
        private readonly Mock<IMoldeRepository> _moldeRepository;
        private readonly Mock<IFasesProducaoRepository> _fpRepository;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IPecaRepository> _pecaRepository;
        private readonly RegistosProducaoService _sut;

        private static Molde MoldeFake(int id = 1) => new()
        {
            Molde_id = id,
            Material = "Aço P20",
            Dimensoes_molde = "300x200x150"
        };

        private static FasesProducao FaseFake(int id = 1) => new()
        {
            Fases_producao_id = id,
            Nome = Nome_fases.MAQUINACAO,
            Descricao = "Maquinação CNC"
        };

        private static User OperadorFake(int id = 1) => new()
        {
            User_id = id,
            Nome = "Operador Teste",
            Email = "op@tipmolde.pt",
            Password = "hash",
            Role = UserRole.OPERADOR
        };

        private static Peca PecaFake(int id = 1, int moldeId = 1) => new()
        {
            Peca_id = id,
            Numero_peca = 1,
            Prioridade = 1,
            Descricao = "Peça de teste",
            Molde_id = moldeId
        };

        private static RegistosProducao RegistoFake(
            int moldeId = 1,
            int faseId = 1,
            int operadorId = 1,
            int pecaId = 1,
            EstadoProducao estado = EstadoProducao.PREPARACAO) => new()
            {
                Molde_id = moldeId,
                Fase_id = faseId,
                Operador_id = operadorId,
                Peca_id = pecaId,
                Maquina = "Fanuc Alpha-T14",
                Estado_producao = estado
            };

        public RegistosProducaoServiceTests()
        {
            _rpRepository = new Mock<IRegistosProducaoRepository>();
            _moldeRepository = new Mock<IMoldeRepository>();
            _fpRepository = new Mock<IFasesProducaoRepository>();
            _userRepository = new Mock<IUserRepository>();
            _pecaRepository = new Mock<IPecaRepository>();

            _sut = new RegistosProducaoService(
                _rpRepository.Object,
                _moldeRepository.Object,
                _fpRepository.Object,
                _userRepository.Object,
                _pecaRepository.Object
            );
        }

        // Configura os mocks comuns a todas as validações de entidades
        private void ConfigurarMocksBase(
            int moldeId = 1,
            int faseId = 1,
            int operadorId = 1,
            int pecaId = 1)
        {
            _moldeRepository.Setup(r => r.GetByIdAsync(moldeId)).ReturnsAsync(MoldeFake(moldeId));
            _fpRepository.Setup(r => r.GetByIdAsync(faseId)).ReturnsAsync(FaseFake(faseId));
            _userRepository.Setup(r => r.GetByIdAsync(operadorId)).ReturnsAsync(OperadorFake(operadorId));
            _pecaRepository.Setup(r => r.GetByIdAsync(pecaId)).ReturnsAsync(PecaFake(pecaId, moldeId));
        }

        // ─────────────────────── CreateRegistoProducaoAsync ────────────────────────────//

        [Fact]
        public async Task CreateRegistoProducaoAsync_ComMoldeInexistente_LancaExcecao()
        {
            _moldeRepository
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Molde?)null);

            var registo = RegistoFake(moldeId: 99);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.CreateRegistoProducaoAsync(registo));
        }

        [Fact]
        public async Task CreateRegistoProducaoAsync_ComFaseInexistente_LancaExcecao()
        {
            _moldeRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MoldeFake());
            _fpRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((FasesProducao?)null);

            var registo = RegistoFake(faseId: 99);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.CreateRegistoProducaoAsync(registo));
        }

        [Fact]
        public async Task CreateRegistoProducaoAsync_ComOperadorInexistente_LancaExcecao()
        {
            _moldeRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MoldeFake());
            _fpRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(FaseFake());
            _userRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

            var registo = RegistoFake(operadorId: 99);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.CreateRegistoProducaoAsync(registo));
        }

        [Fact]
        public async Task CreateRegistoProducaoAsync_ComPecaInexistente_LancaExcecao()
        {
            _moldeRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MoldeFake());
            _fpRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(FaseFake());
            _userRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(OperadorFake());
            _pecaRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Peca?)null);

            var registo = RegistoFake(pecaId: 99);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.CreateRegistoProducaoAsync(registo));
        }


        [Fact]
        public async Task CreateRegistoProducaoAsync_PrimeiroRegisto_EstadoPREPARACAO_Permitido()
        {
            ConfigurarMocksBase();

            _rpRepository
                .Setup(r => r.GetUltimoRegistoAsync(1, 1, 1))
                .ReturnsAsync((RegistosProducao?)null);

            _rpRepository
                .Setup(r => r.AddAsync(It.IsAny<RegistosProducao>()))
                .Returns(Task.CompletedTask);

            var registo = RegistoFake(estado: EstadoProducao.EM_CURSO);

            var resultado = await _sut.CreateRegistoProducaoAsync(registo);

            Assert.Equal(EstadoProducao.EM_CURSO, resultado.Estado_producao);
            _rpRepository.Verify(r => r.AddAsync(It.IsAny<RegistosProducao>()), Times.Once);
        }

        [Theory]
        [InlineData(EstadoProducao.PREPARACAO, EstadoProducao.EM_CURSO)]
        [InlineData(EstadoProducao.EM_CURSO, EstadoProducao.PAUSADO)]
        [InlineData(EstadoProducao.EM_CURSO, EstadoProducao.CONCLUIDO)]
        [InlineData(EstadoProducao.PAUSADO, EstadoProducao.EM_CURSO)]
        public async Task CreateRegistoProducaoAsync_TransicaoValida_CriaRegisto(
            EstadoProducao estadoAtual, EstadoProducao novoEstado)
        {
            ConfigurarMocksBase();

            var ultimo = RegistoFake(estado: estadoAtual);
            ultimo.Registo_Producao_id = 1;
            ultimo.Data_hora = DateTime.UtcNow.AddHours(-1);

            _rpRepository
                .Setup(r => r.GetUltimoRegistoAsync(1, 1, 1))
                .ReturnsAsync(ultimo);

            _rpRepository
                .Setup(r => r.AddAsync(It.IsAny<RegistosProducao>()))
                .Returns(Task.CompletedTask);

            var novo = RegistoFake(estado: novoEstado);

            var resultado = await _sut.CreateRegistoProducaoAsync(novo);

            Assert.Equal(novoEstado, resultado.Estado_producao);
        }

        [Theory]
        [InlineData(EstadoProducao.PREPARACAO, EstadoProducao.PAUSADO)]      // inválido
        [InlineData(EstadoProducao.PREPARACAO, EstadoProducao.CONCLUIDO)]    // inválido
        [InlineData(EstadoProducao.PAUSADO, EstadoProducao.CONCLUIDO)]    // inválido
        [InlineData(EstadoProducao.PAUSADO, EstadoProducao.PREPARACAO)]   // inválido
        [InlineData(EstadoProducao.CONCLUIDO, EstadoProducao.EM_CURSO)]     // inválido
        [InlineData(EstadoProducao.CONCLUIDO, EstadoProducao.PREPARACAO)]   // inválido
        [InlineData(EstadoProducao.EM_CURSO, EstadoProducao.PREPARACAO)]   // inválido
        public async Task CreateRegistoProducaoAsync_TransicaoInvalida_LancaExcecao(
            EstadoProducao estadoAtual, EstadoProducao novoEstado)
        {
            ConfigurarMocksBase();

            var ultimo = RegistoFake(estado: estadoAtual);
            ultimo.Registo_Producao_id = 1;
            ultimo.Data_hora = DateTime.UtcNow.AddHours(-1);

            _rpRepository
                .Setup(r => r.GetUltimoRegistoAsync(1, 1, 1))
                .ReturnsAsync(ultimo);

            var novo = RegistoFake(estado: novoEstado);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.CreateRegistoProducaoAsync(novo));
        }

        [Fact]
        public async Task CreateRegistoProducaoAsync_EstadoCONCLUIDO_NaoPermiteNovosRegistos()
        {
            ConfigurarMocksBase();

            var ultimo = RegistoFake(estado: EstadoProducao.CONCLUIDO);
            ultimo.Registo_Producao_id = 1;
            ultimo.Data_hora = DateTime.UtcNow.AddHours(-1);

            _rpRepository
                .Setup(r => r.GetUltimoRegistoAsync(1, 1, 1))
                .ReturnsAsync(ultimo);

            var novo = RegistoFake(estado: EstadoProducao.EM_CURSO);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.CreateRegistoProducaoAsync(novo));
        }

        [Fact]
        public async Task CreateRegistoProducaoAsync_DataHoraPreenchidaAutomaticamente()
        {
            ConfigurarMocksBase();

            _rpRepository
                .Setup(r => r.GetUltimoRegistoAsync(1, 1, 1))
                .ReturnsAsync((RegistosProducao?)null);

            _rpRepository
                .Setup(r => r.AddAsync(It.IsAny<RegistosProducao>()))
                .Returns(Task.CompletedTask);

            var antes = DateTime.UtcNow;
            var registo = RegistoFake(estado: EstadoProducao.EM_CURSO);
            registo.Data_hora = default;

            var resultado = await _sut.CreateRegistoProducaoAsync(registo);
            var depois = DateTime.UtcNow;

            Assert.True(resultado.Data_hora >= antes && resultado.Data_hora <= depois);
        }

        // ─────────────────────── DeleteRegistoProducaoAsync ────────────────────────────//

        [Fact]
        public async Task DeleteRegistoProducaoAsync_ComIdValido_EliminaRegisto()
        {
            var registo = RegistoFake();
            registo.Registo_Producao_id = 1;

            _rpRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(registo);

            _rpRepository
                .Setup(r => r.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            await _sut.DeleteRegistoProducaoAsync(1);

            _rpRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteRegistoProducaoAsync_ComIdInexistente_LancaExcecao()
        {
            _rpRepository
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((RegistosProducao?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.DeleteRegistoProducaoAsync(999));
        }

        // ──────────── GetHistoricoAsync / GetUltimoRegistoAsync ────────────//


        [Fact]
        public async Task GetHistoricoAsync_ComParametrosValidos_RetornaHistorico()
        {
            var historico = new List<RegistosProducao>
            {
                RegistoFake(estado: EstadoProducao.EM_CURSO),
                RegistoFake(estado: EstadoProducao.PAUSADO),
                RegistoFake(estado: EstadoProducao.CONCLUIDO)
            };

            _rpRepository
                .Setup(r => r.GetHistoricoAsync(1, 1, 1))
                .ReturnsAsync(historico);

            var resultado = await _sut.GetHistoricoAsync(1, 1, 1);

            Assert.Equal(3, resultado.Count());
        }

        [Fact]
        public async Task GetHistoricoAsync_SemRegistos_RetornaListaVazia()
        {
            _rpRepository
                .Setup(r => r.GetHistoricoAsync(1, 1, 1))
                .ReturnsAsync(new List<RegistosProducao>());

            var resultado = await _sut.GetHistoricoAsync(1, 1, 1);

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task GetUltimoRegistoAsync_QuandoExiste_RetornaUltimoRegisto()
        {
            var ultimo = RegistoFake(estado: EstadoProducao.PAUSADO);
            ultimo.Data_hora = DateTime.UtcNow;

            _rpRepository
                .Setup(r => r.GetUltimoRegistoAsync(1, 1, 1))
                .ReturnsAsync(ultimo);

            var resultado = await _sut.GetUltimoRegistoAsync(1, 1, 1);

            Assert.NotNull(resultado);
            Assert.Equal(EstadoProducao.PAUSADO, resultado!.Estado_producao);
        }

        [Fact]
        public async Task GetUltimoRegistoAsync_SemRegistosAnteriores_RetornaNull()
        {
            _rpRepository
                .Setup(r => r.GetUltimoRegistoAsync(1, 1, 1))
                .ReturnsAsync((RegistosProducao?)null);

            var resultado = await _sut.GetUltimoRegistoAsync(1, 1, 1);

            Assert.Null(resultado);
        }
    }*/
}

