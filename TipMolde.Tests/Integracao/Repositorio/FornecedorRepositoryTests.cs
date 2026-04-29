using FluentAssertions;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Infrastructure.Repositorio;

namespace TipMolde.Tests.Integracao.Repositorio
{
    [TestFixture]
    [Category("Integration")]
    public sealed class FornecedorRepositoryTests : RepositoryIntegrationTestBase
    {
        [Test(Description = "TFORREP1 - GetByNif deve devolver fornecedor quando NIF existe.")]
        public async Task GetByNifAsync_Should_ReturnFornecedor_When_NifExists()
        {
            // ARRANGE
            await using var context = CreateContext();
            await context.Fornecedores.AddAsync(new Fornecedor { Nome = "Fornecedor A", NIF = "987654321" });
            await context.SaveChangesAsync();

            var repository = new FornecedorRepository(context);

            // ACT
            var result = await repository.GetByNifAsync("987654321");

            // ASSERT
            result.Should().NotBeNull();
            result!.Nome.Should().Be("Fornecedor A");
        }

        [Test(Description = "TFORREP2 - SearchByName deve normalizar termo e devolver fornecedores ordenados.")]
        public async Task SearchByNameAsync_Should_ReturnOrderedResults_When_SearchTermMatches()
        {
            // ARRANGE
            await using var context = CreateContext();
            await context.Fornecedores.AddRangeAsync(
                new Fornecedor { Nome = "Beta Metais", NIF = "222222222" },
                new Fornecedor { Nome = "Alfa Metais", NIF = "111111111" });
            await context.SaveChangesAsync();

            var repository = new FornecedorRepository(context);

            // ACT
            var result = await repository.SearchByNameAsync(" Metais ", page: 1, pageSize: 10);

            // ASSERT
            result.TotalCount.Should().Be(2);
            result.Items.Select(f => f.Nome).Should().ContainInOrder("Alfa Metais", "Beta Metais");
        }
    }
}
