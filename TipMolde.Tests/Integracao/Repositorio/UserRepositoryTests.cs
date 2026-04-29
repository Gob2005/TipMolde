using FluentAssertions;
using TipMolde.Domain.Entities;
using TipMolde.Domain.Enums;
using TipMolde.Infrastructure.Repositorio;

namespace TipMolde.Tests.Integracao.Repositorio
{
    [TestFixture]
    [Category("Integration")]
    public sealed class UserRepositoryTests : RepositoryIntegrationTestBase
    {
        [Test(Description = "TUSERREP1 - GetByEmail deve devolver utilizador quando email existe.")]
        public async Task GetByEmailAsync_Should_ReturnUser_When_EmailExists()
        {
            // ARRANGE
            await using var context = CreateContext();
            await context.Users.AddAsync(new User
            {
                Nome = "Ana Silva",
                Email = "ana@tipmolde.pt",
                Password = "hash",
                Role = UserRole.ADMIN
            });
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            // ACT
            var result = await repository.GetByEmailAsync("ana@tipmolde.pt");

            // ASSERT
            result.Should().NotBeNull();
            result!.Nome.Should().Be("Ana Silva");
        }

        [Test(Description = "TUSERREP2 - SearchByName deve devolver utilizadores ordenados por nome.")]
        public async Task SearchByNameAsync_Should_ReturnOrderedUsers_When_SearchTermMatches()
        {
            // ARRANGE
            await using var context = CreateContext();
            await context.Users.AddRangeAsync(
                new User { Nome = "Bruno Silva", Email = "bruno@tipmolde.pt", Password = "hash", Role = UserRole.GESTOR_PRODUCAO },
                new User { Nome = "Ana Silva", Email = "ana@tipmolde.pt", Password = "hash", Role = UserRole.ADMIN });
            await context.SaveChangesAsync();

            var repository = new UserRepository(context);

            // ACT
            var result = await repository.SearchByNameAsync("Silva", page: 1, pageSize: 10);

            // ASSERT
            result.TotalCount.Should().Be(2);
            result.Items.Select(u => u.Nome).Should().ContainInOrder("Ana Silva", "Bruno Silva");
        }
    }

}
