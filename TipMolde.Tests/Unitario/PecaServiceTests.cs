using Moq;
using TipMolde.Core.Interface.IMolde;
using TipMolde.Core.Interface.IPeca;
using TipMolde.Core.Models;
using TipMolde.Infrastructure.Service;

namespace TipMolde.Tests.Unitario
{
    /*public class PecaServiceTests
    {
        private readonly Mock<IPecaRepository> _pecaRepository;
        private readonly Mock<IMoldeRepository> _moldeRepository;
        private readonly PecaService _sut;

        private static Molde MoldeFake(int id = 1) => new()
        {
            Molde_id = id,
            Material = "Aço P20",
            Dimensoes_molde = "300x200x150"
        };
        private static Peca PecaFake(int id = 1, int numeroPeca = 10, int moldeId = 1) => new()
        {
            Peca_id = id,
            Numero_peca = numeroPeca,
            Prioridade = 1,
            Descricao = "Peca de teste",
            Molde_id = moldeId
        };

        public PecaServiceTests()
        {
            _pecaRepository = new Mock<IPecaRepository>();
            _moldeRepository = new Mock<IMoldeRepository>();
            _sut = new PecaService(_pecaRepository.Object, _moldeRepository.Object);
        }

        // ────────────────────────── CreatePecaAsync ────────────────────────────────────//

        [Fact]
        public async Task CreatePecaAsync_ComDadosValidos_CriaPeca()
        {
            var molde = MoldeFake();
            var peca = PecaFake();

            _moldeRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(molde);

            _pecaRepository
                .Setup(r => r.GetByNumberAsync(10, 1))
                .ReturnsAsync((Peca?)null);

            _pecaRepository
                .Setup(r => r.AddAsync(It.IsAny<Peca>()))
                .Returns(Task.CompletedTask);

            var resultado = await _sut.CreatePecaAsync(peca);

            Assert.NotNull(resultado);
            Assert.Equal(10, resultado.Numero_peca);
            _pecaRepository.Verify(r => r.AddAsync(It.IsAny<Peca>()), Times.Once);
        }

        [Fact]
        public async Task CreatePecaAsync_ComMoldeInexistente_LancaExcecao()
        {
            _moldeRepository
                .Setup(r => r.GetByIdAsync(99))
                .ReturnsAsync((Molde?)null);

            var peca = PecaFake(moldeId: 99);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.CreatePecaAsync(peca));
        }

        [Fact]
        public async Task CreatePecaAsync_ComNumeroPecaDuplicado_LancaExcecao()
        {
            var molde = MoldeFake();
            var existente = PecaFake(id: 1, numeroPeca: 10, moldeId: 1);

            _moldeRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(molde);

            _pecaRepository
                .Setup(r => r.GetByNumberAsync(10, 1))
                .ReturnsAsync(existente);

            var nova = PecaFake(id: 0, numeroPeca: 10, moldeId: 1);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.CreatePecaAsync(nova));
        }

        // ────────────────────────── UpdatePecaAsync ────────────────────────────────────//

        [Fact]
        public async Task UpdatePecaAsync_ComDadosValidos_AtualizaPeca()
        {
            var existente = PecaFake(id: 1, numeroPeca: 10, moldeId: 1);

            _pecaRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existente);

            _pecaRepository
                .Setup(r => r.UpdateAsync(It.IsAny<Peca>()))
                .Returns(Task.CompletedTask);

            var atualizada = new Peca
            {
                Peca_id = 1,
                Numero_peca = 20,
                Prioridade = 3,
                Descricao = "Descricao atualizada",
                Molde_id = 1
            };

            await _sut.UpdatePecaAsync(atualizada);

            _pecaRepository.Verify(
                r => r.UpdateAsync(It.Is<Peca>(p =>
                    p.Numero_peca == 20 &&
                    p.Prioridade == 3 &&
                    p.Descricao == "Descricao atualizada")),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdatePecaAsync_ComIdInexistente_LancaExcecao()
        {
            _pecaRepository
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Peca?)null);

            var peca = PecaFake(id: 999);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.UpdatePecaAsync(peca));
        }

        [Fact]
        public async Task UpdatePecaAsync_ComNumeroPecaZero_MantemValorOriginal()
        {
            var existente = PecaFake(id: 1, numeroPeca: 1, moldeId: 1);

            _pecaRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existente);

            _pecaRepository
                .Setup(r => r.UpdateAsync(It.IsAny<Peca>()))
                .Returns(Task.CompletedTask);

            var atualizada = new Peca
            {
                Peca_id = 1,
                Numero_peca = 0,
                Prioridade = 0,
                Descricao = null,
                Molde_id = 1
            };

            await _sut.UpdatePecaAsync(atualizada);

            _pecaRepository.Verify(
                r => r.UpdateAsync(It.Is<Peca>(p => p.Numero_peca == 1)),
                Times.Once
            );
        }

        // ────────────────────────── DeletePecaAsync ────────────────────────────────────//

        [Fact]
        public async Task DeletePecaAsync_ComIdValido_EliminaPeca()
        {
            var peca = PecaFake();

            _pecaRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(peca);

            _pecaRepository
                .Setup(r => r.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            await _sut.DeletePecaAsync(1);

            _pecaRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeletePecaAsync_ComIdInexistente_LancaExcecao()
        {
            _pecaRepository
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Peca?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.DeletePecaAsync(999));
        }

        // ──────── GetAllPecasAsync / GetPecaByIdAsync / GetPecaByNumberAsync ───────────────//


        [Fact]
        public async Task GetAllPecasAsync_RetornaTodasAsPecas()
        {
            var lista = new List<Peca>
            {
                PecaFake(id: 1, numeroPeca: 1),
                PecaFake(id: 2, numeroPeca: 2)
            };

            _pecaRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(lista);

            var resultado = await _sut.GetAllPecasAsync();

            Assert.Equal(2, resultado.Count());
        }

        [Fact]
        public async Task GetAllPecasAsync_SemPecas_RetornaListaVazia()
        {
            _pecaRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<Peca>());

            var resultado = await _sut.GetAllPecasAsync();

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task GetPecaByIdAsync_ComIdValido_RetornaPeca()
        {
            var peca = PecaFake(id: 1, numeroPeca: 5, moldeId: 1);

            _pecaRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(peca);

            var resultado = await _sut.GetPecaByIdAsync(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado!.Peca_id);
        }

        [Fact]
        public async Task GetPecaByIdAsync_ComIdInexistente_RetornaNull()
        {
            _pecaRepository
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Peca?)null);

            var resultado = await _sut.GetPecaByIdAsync(999);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task GetPecaByNumberAsync_ComNumerosValidos_RetornaPeca()
        {
            var peca = PecaFake(id: 1, numeroPeca: 5, moldeId: 2);

            _pecaRepository
                .Setup(r => r.GetByNumberAsync(5, 2))
                .ReturnsAsync(peca);

            var resultado = await _sut.GetPecaByNumberAsync(5, 2);

            Assert.NotNull(resultado);
            Assert.Equal(5, resultado!.Numero_peca);
        }

        [Fact]
        public async Task GetPecaByNumberAsync_ComNumerosInexistentes_RetornaNull()
        {
            _pecaRepository
                .Setup(r => r.GetByNumberAsync(999, 999))
                .ReturnsAsync((Peca?)null);

            var resultado = await _sut.GetPecaByNumberAsync(999, 999);

            Assert.Null(resultado);
        }
    }*/
}
