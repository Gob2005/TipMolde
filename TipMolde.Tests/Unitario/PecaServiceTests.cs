using Moq;
using TipMolde.Core.Enums;
using TipMolde.Core.Interface.IMolde;
using TipMolde.Core.Interface.IPeca;
using TipMolde.Core.Models;
using TipMolde.Infrastructure.Service;

namespace TipMolde.Tests.Unitario
{
    public class PecaServiceTests
    {
        private readonly Mock<IPecaRepository> _pecaRepository = new();
        private readonly Mock<IMoldeRepository> _moldeRepository = new();
        private readonly PecaService _sut;

        public PecaServiceTests()
        {
            _sut = new PecaService(_pecaRepository.Object, _moldeRepository.Object);
        }

        private static Molde MoldeFake(int id = 1) => new()
        {
            Molde_id = id,
            Numero = $"M-{id}",
            Numero_cavidades = 1,
            TipoPedido = TipoPedido.NOVO
        };

        private static Peca PecaFake(int id = 1, int moldeId = 1, string designacao = "Extrator") => new()
        {
            Peca_id = id,
            Designacao = designacao,
            Prioridade = 1,
            MaterialDesignacao = "Aco",
            MaterialRecebido = false,
            Molde_id = moldeId
        };

        [Fact]
        public async Task CreatePecaAsync_ValidData_AddsPeca()
        {
            var peca = PecaFake();
            _moldeRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MoldeFake());
            _pecaRepository.Setup(r => r.GetByDesignacaoAsync("Extrator", 1)).ReturnsAsync((Peca?)null);
            _pecaRepository.Setup(r => r.AddAsync(It.IsAny<Peca>())).Returns(Task.CompletedTask);

            var result = await _sut.CreatePecaAsync(peca);

            Assert.Equal("Extrator", result.Designacao);
            _pecaRepository.Verify(r => r.AddAsync(It.IsAny<Peca>()), Times.Once);
        }

        [Fact]
        public async Task CreatePecaAsync_MoldeNotFound_Throws()
        {
            _moldeRepository.Setup(r => r.GetByIdAsync(7)).ReturnsAsync((Molde?)null);
            var peca = PecaFake(moldeId: 7);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.CreatePecaAsync(peca));
        }

        [Fact]
        public async Task CreatePecaAsync_EmptyDesignacao_Throws()
        {
            _moldeRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MoldeFake());
            var peca = PecaFake(designacao: " ");

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreatePecaAsync(peca));
        }

        [Fact]
        public async Task CreatePecaAsync_DuplicateDesignacao_Throws()
        {
            _moldeRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(MoldeFake());
            _pecaRepository.Setup(r => r.GetByDesignacaoAsync("Extrator", 1)).ReturnsAsync(PecaFake(id: 2));
            var peca = PecaFake();

            await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreatePecaAsync(peca));
        }

        [Fact]
        public async Task UpdatePecaAsync_NotFound_Throws()
        {
            _pecaRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Peca?)null);
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdatePecaAsync(PecaFake(id: 99)));
        }

        [Fact]
        public async Task UpdatePecaAsync_ValidData_Updates()
        {
            var existing = PecaFake(id: 1, designacao: "A");
            _pecaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _pecaRepository.Setup(r => r.UpdateAsync(It.IsAny<Peca>())).Returns(Task.CompletedTask);

            var update = PecaFake(id: 1, designacao: "  B  ");
            update.Prioridade = 5;
            update.MaterialDesignacao = "Inox";
            update.MaterialRecebido = true;

            await _sut.UpdatePecaAsync(update);

            _pecaRepository.Verify(r => r.UpdateAsync(It.Is<Peca>(p =>
                p.Peca_id == 1 &&
                p.Designacao == "B" &&
                p.Prioridade == 5 &&
                p.MaterialDesignacao == "Inox" &&
                p.MaterialRecebido)), Times.Once);
        }

        [Fact]
        public async Task UpdatePecaAsync_InvalidIncomingValues_KeepsPrevious()
        {
            var existing = PecaFake(id: 1, designacao: "Original");
            existing.Prioridade = 3;
            existing.MaterialDesignacao = "Aco";
            _pecaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            _pecaRepository.Setup(r => r.UpdateAsync(It.IsAny<Peca>())).Returns(Task.CompletedTask);

            var update = PecaFake(id: 1, designacao: " ");
            update.Prioridade = 0;
            update.MaterialDesignacao = null;
            update.MaterialRecebido = false;

            await _sut.UpdatePecaAsync(update);

            _pecaRepository.Verify(r => r.UpdateAsync(It.Is<Peca>(p =>
                p.Designacao == "Original" &&
                p.Prioridade == 3 &&
                p.MaterialDesignacao == "Aco")), Times.Once);
        }

        [Fact]
        public async Task DeletePecaAsync_NotFound_Throws()
        {
            _pecaRepository.Setup(r => r.GetByIdAsync(10)).ReturnsAsync((Peca?)null);
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeletePecaAsync(10));
        }

        [Fact]
        public async Task DeletePecaAsync_Found_Deletes()
        {
            _pecaRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(PecaFake());
            _pecaRepository.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

            await _sut.DeletePecaAsync(1);

            _pecaRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetByDesignacaoAsync_DelegatesToRepository()
        {
            _pecaRepository.Setup(r => r.GetByDesignacaoAsync("Extrator", 2)).ReturnsAsync(PecaFake(id: 5, moldeId: 2));

            var result = await _sut.GetByDesignacaoAsync("Extrator", 2);

            Assert.NotNull(result);
            Assert.Equal(5, result!.Peca_id);
        }
    }
}
