using FluentAssertions;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Enums;
using TipMolde.Infrastructure.Repositorio;

namespace TipMolde.Tests.Integracao.Repositorio
{
    [TestFixture]
    [Category("Integration")]
    public sealed class EncomendaRepositoryTests : RepositoryIntegrationTestBase
    {
        [Test(Description = "TENCREP1 - GetEncomendasPorConcluir deve excluir encomendas concluidas e canceladas.")]
        public async Task GetEncomendasPorConcluirAsync_Should_ExcludeClosedStates()
        {
            // ARRANGE
            await using var context = CreateContext();
            await context.Encomendas.AddRangeAsync(
                new Encomenda { NumeroEncomendaCliente = "ENC-001", Estado = EstadoEncomenda.CONFIRMADA },
                new Encomenda { NumeroEncomendaCliente = "ENC-002", Estado = EstadoEncomenda.CONCLUIDA },
                new Encomenda { NumeroEncomendaCliente = "ENC-003", Estado = EstadoEncomenda.CANCELADA });
            await context.SaveChangesAsync();

            var repository = new EncomendaRepository(context);

            // ACT
            var result = await repository.GetEncomendasPorConcluirAsync(page: 1, pageSize: 10);

            // ASSERT
            result.Items.Should().ContainSingle(e => e.NumeroEncomendaCliente == "ENC-001");
        }

        [Test(Description = "TENCREP2 - GetByNumeroEncomendaCliente deve procurar numero normalizado.")]
        public async Task GetByNumeroEncomendaClienteAsync_Should_ReturnEncomenda_When_NumeroHasSpaces()
        {
            // ARRANGE
            await using var context = CreateContext();
            await context.Encomendas.AddAsync(new Encomenda { NumeroEncomendaCliente = "ENC-001" });
            await context.SaveChangesAsync();

            var repository = new EncomendaRepository(context);

            // ACT
            var result = await repository.GetByNumeroEncomendaClienteAsync(" ENC-001 ");

            // ASSERT
            result.Should().NotBeNull();
            result!.NumeroEncomendaCliente.Should().Be("ENC-001");
        }
    }
}
