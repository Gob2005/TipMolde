using FluentAssertions;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Infrastructure.Repositorio;

namespace TipMolde.Tests.Integracao.Repositorio;

[TestFixture]
[Category("Integration")]
public sealed class GenericRepositoryTests : RepositoryIntegrationTestBase
{
    [Test(Description = "TGENREP1 - GetAllAsync deve devolver pagina solicitada com total de registos.")]
    public async Task GetAllAsync_Should_ReturnRequestedPageWithTotalCount()
    {
        // ARRANGE
        await using var context = CreateContext();
        await context.Clientes.AddRangeAsync(
            new Cliente { Nome = "Cliente 1", NIF = "111111111", Sigla = "C01" },
            new Cliente { Nome = "Cliente 2", NIF = "222222222", Sigla = "C02" },
            new Cliente { Nome = "Cliente 3", NIF = "333333333", Sigla = "C03" });
        await context.SaveChangesAsync();

        var repository = new GenericRepository<Cliente, int>(context);

        // ACT
        var result = await repository.GetAllAsync(page: 2, pageSize: 1);

        // ASSERT
        result.TotalCount.Should().Be(3);
        result.CurrentPage.Should().Be(2);
        result.PageSize.Should().Be(1);
        result.Items.Should().ContainSingle();
    }

    [Test(Description = "TGENREP2 - AddAsync e GetByIdAsync devem persistir e obter entidade por chave.")]
    public async Task AddAsyncAndGetByIdAsync_Should_PersistAndReturnEntity()
    {
        // ARRANGE
        await using var context = CreateContext();
        var repository = new GenericRepository<Cliente, int>(context);

        // ACT
        var created = await repository.AddAsync(new Cliente { Nome = "Cliente A", NIF = "444444444", Sigla = "CLA" });
        var result = await repository.GetByIdAsync(created.Cliente_id);

        // ASSERT
        created.Cliente_id.Should().BeGreaterThan(0);
        result.Should().NotBeNull();
        result!.Nome.Should().Be("Cliente A");
    }

    [Test(Description = "TGENREP3 - UpdateAsync deve persistir alteracoes da entidade.")]
    public async Task UpdateAsync_Should_PersistEntityChanges()
    {
        // ARRANGE
        await using var context = CreateContext();
        var cliente = new Cliente { Nome = "Cliente Inicial", NIF = "555555555", Sigla = "CLI" };
        await context.Clientes.AddAsync(cliente);
        await context.SaveChangesAsync();

        var repository = new GenericRepository<Cliente, int>(context);
        cliente.Nome = "Cliente Atualizado";

        // ACT
        await repository.UpdateAsync(cliente);

        // ASSERT
        context.Clientes.Single(c => c.Cliente_id == cliente.Cliente_id).Nome.Should().Be("Cliente Atualizado");
    }

    [Test(Description = "TGENREP4 - DeleteAsync deve remover entidade quando existe e ignorar chave inexistente.")]
    public async Task DeleteAsync_Should_RemoveEntityOrIgnoreMissingKey()
    {
        // ARRANGE
        await using var context = CreateContext();
        var cliente = new Cliente { Nome = "Cliente Remover", NIF = "666666666", Sigla = "REM" };
        await context.Clientes.AddAsync(cliente);
        await context.SaveChangesAsync();

        var repository = new GenericRepository<Cliente, int>(context);

        // ACT
        await repository.DeleteAsync(cliente.Cliente_id);
        await repository.DeleteAsync(999);

        // ASSERT
        context.Clientes.Should().BeEmpty();
    }
}
