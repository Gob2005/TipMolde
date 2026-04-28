using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;
using TipMolde.Infrastructure.DB;
using TipMolde.Infrastructure.Repositorio;

namespace TipMolde.Tests.Integracao.Repositorio;

[TestFixture]
[Category("Integration")]
public class FasesProducaoRepositoryTests
{
    private static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    [Test(Description = "TFPREP1 - GetByNome deve devolver a fase quando o nome existe.")]
    public async Task GetByNomeAsync_Should_ReturnFase_When_NomeExists()
    {
        // ARRANGE
        await using var context = CreateContext();
        await context.Fases_Producao.AddAsync(new FasesProducao
        {
            Nome = NomeFases.MAQUINACAO,
            Descricao = "Descricao"
        });
        await context.SaveChangesAsync();

        var repository = new FasesProducaoRepository(context);

        // ACT
        var result = await repository.GetByNomeAsync(NomeFases.MAQUINACAO);

        // ASSERT
        result.Should().NotBeNull();
        result!.Nome.Should().Be(NomeFases.MAQUINACAO);
    }

    [Test(Description = "TFPREP2 - HasMaquinasAssociadas deve devolver true quando existe pelo menos uma maquina na fase.")]
    public async Task HasMaquinasAssociadasAsync_Should_ReturnTrue_When_FaseIsReferencedByMaquina()
    {
        // ARRANGE
        await using var context = CreateContext();

        var fase = new FasesProducao
        {
            Nome = NomeFases.EROSAO,
            Descricao = "Descricao"
        };

        await context.Fases_Producao.AddAsync(fase);
        await context.SaveChangesAsync();

        await context.Maquinas.AddAsync(new Maquina
        {
            Numero = 10,
            NomeModelo = "Makino",
            FaseDedicada_id = fase.Fases_producao_id
        });
        await context.SaveChangesAsync();

        var repository = new FasesProducaoRepository(context);

        // ACT
        var result = await repository.HasMaquinasAssociadasAsync(fase.Fases_producao_id);

        // ASSERT
        result.Should().BeTrue();
    }
}
