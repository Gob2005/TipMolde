using FluentAssertions;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;
using TipMolde.Infrastructure.Repositorio;

namespace TipMolde.Tests.Integracao.Repositorio
{
    [TestFixture]
    [Category("Integration")]
    public sealed class MaquinaRepositoryTests : RepositoryIntegrationTestBase
    {
        [Test(Description = "TMAQREP1 - GetByEstado deve devolver maquinas filtradas e ordenadas por numero.")]
        public async Task GetByEstadoAsync_Should_ReturnOrderedResults_When_EstadoMatches()
        {
            // ARRANGE
            await using var context = CreateContext();
            await context.Maquinas.AddRangeAsync(
                new Maquina { Numero = 20, NomeModelo = "Makino", Estado = EstadoMaquina.DISPONIVEL, FaseDedicada_id = 1 },
                new Maquina { Numero = 10, NomeModelo = "Fanuc", Estado = EstadoMaquina.DISPONIVEL, FaseDedicada_id = 1 },
                new Maquina { Numero = 30, NomeModelo = "Charmilles", Estado = EstadoMaquina.MANUTENCAO, FaseDedicada_id = 1 });
            await context.SaveChangesAsync();

            var repository = new MaquinaRepository(context);

            // ACT
            var result = await repository.GetByEstadoAsync(EstadoMaquina.DISPONIVEL, page: 1, pageSize: 10);

            // ASSERT
            result.TotalCount.Should().Be(2);
            result.Items.Select(m => m.Numero).Should().ContainInOrder(10, 20);
        }

        [Test(Description = "TMAQREP2 - ExistsNumero deve devolver false quando numero pertence a maquina excluida.")]
        public async Task ExistsNumeroAsync_Should_ReturnFalse_When_ExistingMachineIsExcluded()
        {
            // ARRANGE
            await using var context = CreateContext();
            var maquina = new Maquina { Numero = 10, NomeModelo = "Makino", FaseDedicada_id = 1 };
            await context.Maquinas.AddAsync(maquina);
            await context.SaveChangesAsync();

            var repository = new MaquinaRepository(context);

            // ACT
            var result = await repository.ExistsNumeroAsync(10, maquina.Maquina_id);

            // ASSERT
            result.Should().BeFalse();
        }

        [Test(Description = "TMAQREP3 - GetByIdUnico deve devolver maquina quando ID existe.")]
        public async Task GetByIdUnicoAsync_Should_ReturnMaquina_When_IdExists()
        {
            // ARRANGE
            await using var context = CreateContext();
            var maquina = new Maquina { Numero = 40, NomeModelo = "Makino", FaseDedicada_id = 1 };
            await context.Maquinas.AddAsync(maquina);
            await context.SaveChangesAsync();

            var repository = new MaquinaRepository(context);

            // ACT
            var result = await repository.GetByIdUnicoAsync(maquina.Maquina_id);

            // ASSERT
            result.Should().NotBeNull();
            result!.Numero.Should().Be(40);
        }

        [Test(Description = "TMAQREP4 - ExistsNumero deve devolver true quando numero ja existe.")]
        public async Task ExistsNumeroAsync_Should_ReturnTrue_When_NumeroExists()
        {
            // ARRANGE
            await using var context = CreateContext();
            await context.Maquinas.AddAsync(new Maquina { Numero = 10, NomeModelo = "Makino", FaseDedicada_id = 1 });
            await context.SaveChangesAsync();

            var repository = new MaquinaRepository(context);

            // ACT
            var result = await repository.ExistsNumeroAsync(10);

            // ASSERT
            result.Should().BeTrue();
        }

        [Test(Description = "TMAQREP5 - ExistsFaseDedicada deve indicar se fase existe.")]
        public async Task ExistsFaseDedicadaAsync_Should_ReturnExpectedValue_When_FaseExistsOrNot()
        {
            // ARRANGE
            await using var context = CreateContext();
            var fase = new FasesProducao { Nome = NomeFases.MAQUINACAO, Descricao = "Maquinacao" };
            await context.Fases_Producao.AddAsync(fase);
            await context.SaveChangesAsync();

            var repository = new MaquinaRepository(context);

            // ACT
            var exists = await repository.ExistsFaseDedicadaAsync(fase.Fases_producao_id);
            var missing = await repository.ExistsFaseDedicadaAsync(999);

            // ASSERT
            exists.Should().BeTrue();
            missing.Should().BeFalse();
        }

        [Test(Description = "TMAQREP6 - CreateAsync e UpdateExistingAsync devem persistir maquina e alteracoes.")]
        public async Task CreateAsyncAndUpdateExistingAsync_Should_PersistMaquinaAndChanges()
        {
            // ARRANGE
            await using var context = CreateContext();
            var repository = new MaquinaRepository(context);
            var maquina = new Maquina { Numero = 50, NomeModelo = "Fanuc", FaseDedicada_id = 1 };

            // ACT
            var created = await repository.CreateAsync(maquina);
            created.Estado = EstadoMaquina.MANUTENCAO;
            await repository.UpdateExistingAsync(created);

            // ASSERT
            created.Maquina_id.Should().BeGreaterThan(0);
            context.Maquinas.Single(m => m.Maquina_id == created.Maquina_id).Estado.Should().Be(EstadoMaquina.MANUTENCAO);
        }
    }
}
