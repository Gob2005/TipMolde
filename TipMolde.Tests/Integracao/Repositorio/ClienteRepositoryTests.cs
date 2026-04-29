using FluentAssertions;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Infrastructure.Repositorio;

namespace TipMolde.Tests.Integracao.Repositorio
{
    [TestFixture]
    [Category("Integration")]
    public sealed class ClienteRepositoryTests : RepositoryIntegrationTestBase
    {
        [Test(Description = "TCLIREP1 - GetClienteWithEncomendas deve carregar cliente com encomendas associadas.")]
        public async Task GetClienteWithEncomendasAsync_Should_LoadEncomendas_When_ClienteExists()
        {
            // ARRANGE
            await using var context = CreateContext();
            var cliente = new Cliente { Nome = "Cliente A", NIF = "123456789", Sigla = "CLA" };
            cliente.Encomendas.Add(new Encomenda { NumeroEncomendaCliente = "ENC-001" });

            await context.Clientes.AddAsync(cliente);
            await context.SaveChangesAsync();

            var repository = new ClienteRepository(context);

            // ACT
            var result = await repository.GetClienteWithEncomendasAsync(cliente.Cliente_id);

            // ASSERT
            result.Should().NotBeNull();
            result!.Encomendas.Should().ContainSingle(e => e.NumeroEncomendaCliente == "ENC-001");
        }

        [Test(Description = "TCLIREP2 - SearchBySigla deve devolver resultados paginados ordenados por sigla.")]
        public async Task SearchBySiglaAsync_Should_ReturnPagedResults_When_SearchTermMatches()
        {
            // ARRANGE
            await using var context = CreateContext();
            await context.Clientes.AddRangeAsync(
                new Cliente { Nome = "Cliente B", NIF = "222222222", Sigla = "BTA" },
                new Cliente { Nome = "Cliente A", NIF = "111111111", Sigla = "ATA" });
            await context.SaveChangesAsync();

            var repository = new ClienteRepository(context);

            // ACT
            var result = await repository.SearchBySiglaAsync("TA", page: 1, pageSize: 10);

            // ASSERT
            result.TotalCount.Should().Be(2);
            result.Items.Select(c => c.Sigla).Should().ContainInOrder("ATA", "BTA");
        }
    }
}
