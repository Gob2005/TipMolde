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

        [Test(Description = "TCLIREP3 - GetByNif deve devolver cliente sem tracking quando NIF existe.")]
        public async Task GetByNifAsync_Should_ReturnCliente_When_NifExists()
        {
            // ARRANGE
            await using var context = CreateContext();
            await context.Clientes.AddAsync(new Cliente { Nome = "Cliente NIF", NIF = "333333333", Sigla = "NIF" });
            await context.SaveChangesAsync();

            var repository = new ClienteRepository(context);

            // ACT
            var result = await repository.GetByNifAsync("333333333");

            // ASSERT
            result.Should().NotBeNull();
            result!.Nome.Should().Be("Cliente NIF");
        }

        [Test(Description = "TCLIREP4 - GetBySigla deve devolver cliente quando sigla existe.")]
        public async Task GetBySiglaAsync_Should_ReturnCliente_When_SiglaExists()
        {
            // ARRANGE
            await using var context = CreateContext();
            await context.Clientes.AddAsync(new Cliente { Nome = "Cliente Sigla", NIF = "444444444", Sigla = "SIG" });
            await context.SaveChangesAsync();

            var repository = new ClienteRepository(context);

            // ACT
            var result = await repository.GetBySiglaAsync("SIG");

            // ASSERT
            result.Should().NotBeNull();
            result!.NIF.Should().Be("444444444");
        }

        [Test(Description = "TCLIREP5 - SearchByName deve devolver pagina solicitada ordenada por nome.")]
        public async Task SearchByNameAsync_Should_ReturnRequestedPageOrderedByName()
        {
            // ARRANGE
            await using var context = CreateContext();
            await context.Clientes.AddRangeAsync(
                new Cliente { Nome = "Zeta Moldes", NIF = "555555555", Sigla = "ZET" },
                new Cliente { Nome = "Alfa Moldes", NIF = "666666666", Sigla = "ALF" },
                new Cliente { Nome = "Beta Moldes", NIF = "777777777", Sigla = "BET" });
            await context.SaveChangesAsync();

            var repository = new ClienteRepository(context);

            // ACT
            var result = await repository.SearchByNameAsync("Moldes", page: 2, pageSize: 1);

            // ASSERT
            result.TotalCount.Should().Be(3);
            result.CurrentPage.Should().Be(2);
            result.Items.Should().ContainSingle(c => c.Nome == "Beta Moldes");
        }
    }
}
