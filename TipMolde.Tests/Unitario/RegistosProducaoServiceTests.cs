using Moq;
using TipMolde.Core.Enums;
using TipMolde.Core.Interface.Producao.IFasesProducao;
using TipMolde.Core.Interface.Producao.IMaquina;
using TipMolde.Core.Interface.Producao.IPeca;
using TipMolde.Core.Interface.Producao.IRegistosProducao;
using TipMolde.Core.Interface.Utilizador.IUser;
using TipMolde.Core.Models;
using TipMolde.Core.Models.Producao;
using TipMolde.Infrastructure.Service;

namespace TipMolde.Tests.Unitario
{
    /*public class RegistosProducaoServiceTests
    {
        private readonly Mock<IRegistosProducaoRepository> _rpRepository = new();
        private readonly Mock<IFasesProducaoRepository> _fpRepository = new();
        private readonly Mock<IUserRepository> _userRepository = new();
        private readonly Mock<IPecaRepository> _pecaRepository = new();
        private readonly Mock<IMaquinaRepository> _maquinaRepository = new();

        private readonly RegistosProducaoService _sut;

        public RegistosProducaoServiceTests()
        {
            _sut = new RegistosProducaoService(
                _rpRepository.Object,
                _fpRepository.Object,
                _userRepository.Object,
                _maquinaRepository.Object,
                _pecaRepository.Object);
        }

        private static FasesProducao FaseFake(int id = 1) => new()
        {
            Fases_producao_id = id,
            Nome = Nome_fases.MAQUINACAO,
            Descricao = "Fase de teste"
        };

        private static User OperadorFake(int id = 1) => new()
        {
            User_id = id,
            Nome = "Operador Teste",
            Email = "op@tipmolde.pt",
            Password = "hash",
            Role = UserRole.GESTOR_PRODUCAO
        };

        private static Peca PecaFake(int id = 1) => new()
        {
            Peca_id = id,
            Designacao = "Peca Teste",
            Prioridade = 1,
            Molde_id = 1
        };

        private static RegistosProducao RegistoFake(
            int faseId = 1,
            int operadorId = 1,
            int pecaId = 1,
            EstadoProducao estado = EstadoProducao.PREPARACAO) => new()
            {
                Fase_id = faseId,
                Operador_id = operadorId,
                Peca_id = pecaId,
                Estado_producao = estado,
                Maquina_id = 1
            };

        [Fact]
        public async Task CreateRegistoProducaoAsync_FaseNotFound_Throws()
        {
            _fpRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((FasesProducao?)null);

            var registo = RegistoFake(faseId: 99);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.CreateRegistoProducaoAsync(registo));
        }

        [Fact]
        public async Task CreateRegistoProducaoAsync_OperadorNotFound_Throws()
        {
            _fpRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(FaseFake());
            _userRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

            var registo = RegistoFake(operadorId: 99);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.CreateRegistoProducaoAsync(registo));
        }

        [Fact]
        public async Task CreateRegistoProducaoAsync_PecaNotFound_Throws()
        {
            _fpRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(FaseFake());
            _userRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(OperadorFake());
            _pecaRepository.Setup(r => r.GetByIdAsync(77)).ReturnsAsync((Peca?)null);

            var registo = RegistoFake(pecaId: 77);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.CreateRegistoProducaoAsync(registo));
        }

        [Fact]
        public async Task CreateRegistoProducaoAsync_FirstStatePreparacao_Succeeds()
        {
            _fpRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(FaseFake());
            _userRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(OperadorFake());
            _pecaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(PecaFake());
            _rpRepository.Setup(r => r.GetUltimoRegistoAsync(1, 1)).ReturnsAsync((RegistosProducao?)null);
            _rpRepository.Setup(r => r.AddAsync(It.IsAny<RegistosProducao>())).Returns(Task.CompletedTask);

            var registo = RegistoFake(estado: EstadoProducao.PREPARACAO);
            var before = DateTime.UtcNow;

            var result = await _sut.CreateRegistoProducaoAsync(registo);

            Assert.Equal(EstadoProducao.PREPARACAO, result.Estado_producao);
            Assert.True(result.Data_hora >= before);
            _rpRepository.Verify(r => r.AddAsync(It.IsAny<RegistosProducao>()), Times.Once);
        }

        [Fact]
        public async Task CreateRegistoProducaoAsync_InvalidFirstTransition_Throws()
        {
            _fpRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(FaseFake());
            _userRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(OperadorFake());
            _pecaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(PecaFake());
            _rpRepository.Setup(r => r.GetUltimoRegistoAsync(1, 1)).ReturnsAsync((RegistosProducao?)null);

            var registo = RegistoFake(estado: EstadoProducao.EM_CURSO);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateRegistoProducaoAsync(registo));
        }

        [Fact]
        public async Task CreateRegistoProducaoAsync_ValidTransition_PreparacaoToEmCurso_Succeeds()
        {
            _fpRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(FaseFake());
            _userRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(OperadorFake());
            _pecaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(PecaFake());
            _rpRepository.Setup(r => r.GetUltimoRegistoAsync(1, 1)).ReturnsAsync(new RegistosProducao
            {
                Fase_id = 1,
                Peca_id = 1,
                Estado_producao = EstadoProducao.PREPARACAO,
                Data_hora = DateTime.UtcNow.AddMinutes(-10)
            });
            _rpRepository.Setup(r => r.AddAsync(It.IsAny<RegistosProducao>())).Returns(Task.CompletedTask);

            var registo = RegistoFake(estado: EstadoProducao.EM_CURSO);

            var result = await _sut.CreateRegistoProducaoAsync(registo);

            Assert.Equal(EstadoProducao.EM_CURSO, result.Estado_producao);
        }

        [Fact]
        public async Task CreateRegistoProducaoAsync_InvalidTransition_PreparacaoToConcluido_Throws()
        {
            _fpRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(FaseFake());
            _userRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(OperadorFake());
            _pecaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(PecaFake());
            _rpRepository.Setup(r => r.GetUltimoRegistoAsync(1, 1)).ReturnsAsync(new RegistosProducao
            {
                Fase_id = 1,
                Peca_id = 1,
                Estado_producao = EstadoProducao.PREPARACAO,
                Data_hora = DateTime.UtcNow.AddMinutes(-10)
            });

            var registo = RegistoFake(estado: EstadoProducao.CONCLUIDO);

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateRegistoProducaoAsync(registo));
        }

        [Fact]
        public async Task DeleteRegistoProducaoAsync_NotFound_Throws()
        {
            _rpRepository.Setup(r => r.GetByIdAsync(10)).ReturnsAsync((RegistosProducao?)null);
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteRegistoProducaoAsync(10));
        }

        [Fact]
        public async Task DeleteRegistoProducaoAsync_Found_Deletes()
        {
            _rpRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new RegistosProducao { Registo_Producao_id = 1 });
            _rpRepository.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            await _sut.DeleteRegistoProducaoAsync(1);

            _rpRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }
    }*/
}
