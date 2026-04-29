using Microsoft.EntityFrameworkCore;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Tests.Integracao.Repositorio;

/// <summary>
/// Base comum para testes de integracao de repositorios.
/// </summary>
/// <remarks>
/// Cria uma base InMemory isolada por teste, seguindo o padrao ja usado nos
/// testes de repositorio existentes do projeto.
/// </remarks>
public abstract class RepositoryIntegrationTestBase
{
    protected static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }
}
