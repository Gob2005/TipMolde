using Moq;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Producao.IFasesProducao;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;
using TipMolde.Infrastructure.Service;

namespace TipMolde.Tests.Unitario
{
    public class FasesProducaoServiceTests
    {
        private readonly Mock<IFasesProducaoRepository> _fpRepository;
        private readonly FasesProducaoService _sut;

        private static FasesProducao FaseFake(
            int id = 1,
            Nome_fases nome = Nome_fases.MAQUINACAO,
            string descricao = "Fase de maquinação") => new()
            {
                Fases_producao_id = id,
                Nome = nome,
                Descricao = descricao
            };

        public FasesProducaoServiceTests()
        {
            _fpRepository = new Mock<IFasesProducaoRepository>();
            _sut = new FasesProducaoService(_fpRepository.Object);
        }

        // ─────────────────────── CreateFase_producaoAsync ────────────────────────────//

        [Fact]
        public async Task CreateFase_producaoAsync_ComNomeNovo_CriaFase()
        {
            var fase = FaseFake();

            _fpRepository
                .Setup(r => r.GetByNomeAsync(Nome_fases.MAQUINACAO))
                .ReturnsAsync((FasesProducao?)null);

            _fpRepository
                .Setup(r => r.AddAsync(It.IsAny<FasesProducao>()))
                .ReturnsAsync((FasesProducao f) => f);

            var resultado = await _sut.CreateAsync(fase);

            Assert.NotNull(resultado);
            Assert.Equal(Nome_fases.MAQUINACAO, resultado.Nome);
            _fpRepository.Verify(r => r.AddAsync(It.IsAny<FasesProducao>()), Times.Once);
        }

        [Fact]
        public async Task CreateFase_producaoAsync_ComNomeDuplicado_LancaExcecao()
        {
            var existente = FaseFake();

            _fpRepository
                .Setup(r => r.GetByNomeAsync(Nome_fases.MAQUINACAO))
                .ReturnsAsync(existente);

            var nova = FaseFake(id: 0);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.CreateAsync(nova));
        }

        [Theory]
        [InlineData(Nome_fases.MAQUINACAO)]
        [InlineData(Nome_fases.EROSAO)]
        [InlineData(Nome_fases.MONTAGEM)]
        public async Task CreateFase_producaoAsync_ParaCadaNomeFase_CriaComSucesso(Nome_fases nome)
        {
            _fpRepository
                .Setup(r => r.GetByNomeAsync(nome))
                .ReturnsAsync((FasesProducao?)null);

            _fpRepository
                .Setup(r => r.AddAsync(It.IsAny<FasesProducao>()))
                .ReturnsAsync((FasesProducao f) => f);

            var fase = new FasesProducao { Nome = nome, Descricao = "Descrição de teste" };

            var resultado = await _sut.CreateAsync(fase);

            Assert.Equal(nome, resultado.Nome);
        }

        // ─────────────────────── UpdateFase_producaoAsync ────────────────────────────//

        [Fact]
        public async Task UpdateFase_producaoAsync_ComDadosValidos_AtualizaFase()
        {
            var existente = FaseFake(id: 1, nome: Nome_fases.MAQUINACAO, descricao: "Antiga");

            _fpRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existente);

            _fpRepository
                .Setup(r => r.GetByNomeAsync(Nome_fases.EROSAO))
                .ReturnsAsync((FasesProducao?)null);

            _fpRepository
                .Setup(r => r.UpdateAsync(It.IsAny<FasesProducao>()))
                .Returns(Task.CompletedTask);

            var atualizada = new FasesProducao
            {
                Fases_producao_id = 1,
                Nome = Nome_fases.EROSAO,
                Descricao = "Descrição atualizada"
            };

            await _sut.UpdateAsync(atualizada);

            _fpRepository.Verify(
                r => r.UpdateAsync(It.Is<FasesProducao>(f =>
                    f.Nome == Nome_fases.EROSAO &&
                    f.Descricao == "Descrição atualizada")),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateFase_producaoAsync_ComIdInexistente_LancaExcecao()
        {
            _fpRepository
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((FasesProducao?)null);

            var fase = FaseFake(id: 999);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.UpdateAsync(fase));
        }

        [Fact]
        public async Task UpdateFase_producaoAsync_ComNomeJaUsadoPorOutraFase_LancaExcecao()
        {
            var existente = FaseFake(id: 1, nome: Nome_fases.MAQUINACAO);
            var outra = FaseFake(id: 2, nome: Nome_fases.EROSAO);

            _fpRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existente);

            _fpRepository
                .Setup(r => r.GetByNomeAsync(Nome_fases.EROSAO))
                .ReturnsAsync(outra); 

            var atualizada = new FasesProducao
            {
                Fases_producao_id = 1,
                Nome = Nome_fases.EROSAO,
                Descricao = "Sem conflito na descrição"
            };

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.UpdateAsync(atualizada));
        }

        [Fact]
        public async Task UpdateFase_producaoAsync_SemAlteracaoDeNome_AtualizaApenasDescricao()
        {
            var existente = FaseFake(id: 1, nome: Nome_fases.MAQUINACAO, descricao: "Antiga");

            _fpRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existente);

            _fpRepository
                .Setup(r => r.UpdateAsync(It.IsAny<FasesProducao>()))
                .Returns(Task.CompletedTask);

            var atualizada = new FasesProducao
            {
                Fases_producao_id = 1,
                Nome = Nome_fases.MAQUINACAO,
                Descricao = "Descrição nova"
            };

            await _sut.UpdateAsync(atualizada);

            _fpRepository.Verify(
                r => r.UpdateAsync(It.Is<FasesProducao>(f =>
                    f.Nome == Nome_fases.MAQUINACAO &&
                    f.Descricao == "Descrição nova")),
                Times.Once
            );
        }

        // ─────────────────────── DeleteFase_producaoAsync ────────────────────────────//

        [Fact]
        public async Task DeleteFase_producaoAsync_ComIdValido_EliminaFase()
        {
            var fase = FaseFake();

            _fpRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(fase);

            _fpRepository
                .Setup(r => r.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            await _sut.DeleteAsync(1);

            _fpRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteFase_producaoAsync_ComIdInexistente_LancaExcecao()
        {
            _fpRepository
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((FasesProducao?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.DeleteAsync(999));
        }

        // ─────────────────────── GetAllFases / GetByIdAsync ────────────────────────────//

        [Fact]
        public async Task GetAllFases_producaoAsync_RetornaTodasAsFases()
        {
            var lista = new List<FasesProducao>
            {
                FaseFake(1, Nome_fases.MAQUINACAO),
                FaseFake(2, Nome_fases.EROSAO),
                FaseFake(3, Nome_fases.MONTAGEM)
            };

            _fpRepository
                .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new PagedResult<FasesProducao>(
                    lista,
                    lista.Count,
                    1,
                    lista.Count
                ));

            var resultado = await _sut.GetAllAsync();

            Assert.Equal(3, resultado.Items.Count());
        }

        [Fact]
        public async Task GetAllFases_producaoAsync_SemFases_RetornaListaVazia()
        {
            _fpRepository
                .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new PagedResult<FasesProducao>(
                    Enumerable.Empty<FasesProducao>(),
                    0,
                    1,
                    50
                ));

            var resultado = await _sut.GetAllAsync();

            Assert.Empty(resultado.Items);
        }

        [Fact]
        public async Task GetFase_producaoByIdAsync_ComIdValido_RetornaFase()
        {
            var fase = FaseFake(id: 1);

            _fpRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(fase);

            var resultado = await _sut.GetByIdAsync(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado!.Fases_producao_id);
        }

        [Fact]
        public async Task GetFase_producaoByIdAsync_ComIdInexistente_RetornaNull()
        {
            _fpRepository
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((FasesProducao?)null);

            var resultado = await _sut.GetByIdAsync(999);

            Assert.Null(resultado);
        }
    }
}