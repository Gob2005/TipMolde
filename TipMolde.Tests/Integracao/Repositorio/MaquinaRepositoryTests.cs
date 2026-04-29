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
    }
}
