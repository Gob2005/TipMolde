using FluentAssertions;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;
using TipMolde.Infrastructure.Repositorio;

namespace TipMolde.Tests.Integracao.Repositorio;

[TestFixture]
[Category("Integration")]
public sealed class FasesProducaoRepositoryTests : RepositoryIntegrationTestBase
{
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

    [Test(Description = "TFPREP3 - HasMaquinasAssociadas deve devolver false quando a fase nao tem maquinas.")]
    public async Task HasMaquinasAssociadasAsync_Should_ReturnFalse_When_FaseHasNoMaquinas()
    {
        // ARRANGE
        await using var context = CreateContext();

        var fase = new FasesProducao
        {
            Nome = NomeFases.MONTAGEM,
            Descricao = "Montagem final"
        };

        await context.Fases_Producao.AddAsync(fase);
        await context.SaveChangesAsync();

        var repository = new FasesProducaoRepository(context);

        // ACT
        var result = await repository.HasMaquinasAssociadasAsync(fase.Fases_producao_id);

        // ASSERT
        result.Should().BeFalse();
    }

    [Test(Description = "TFPREP4 - CreateAsync deve persistir nova fase de producao.")]
    public async Task CreateAsync_Should_PersistFase_When_DataIsValid()
    {
        // ARRANGE
        await using var context = CreateContext();
        var repository = new FasesProducaoRepository(context);
        var fase = new FasesProducao
        {
            Nome = NomeFases.MAQUINACAO,
            Descricao = "Acabamento final"
        };

        // ACT
        var result = await repository.CreateAsync(fase);

        // ASSERT
        result.Fases_producao_id.Should().BeGreaterThan(0);
        context.Fases_Producao.Should().ContainSingle(f => f.Nome == NomeFases.MAQUINACAO);
    }

    [Test(Description = "TFPREP5 - UpdateExistingAsync deve persistir alteracoes na fase existente.")]
    public async Task UpdateExistingAsync_Should_PersistChanges_When_FaseExists()
    {
        // ARRANGE
        await using var context = CreateContext();
        var fase = new FasesProducao
        {
            Nome = NomeFases.EROSAO,
            Descricao = "Descricao inicial"
        };

        await context.Fases_Producao.AddAsync(fase);
        await context.SaveChangesAsync();

        fase.Descricao = "Descricao atualizada";
        var repository = new FasesProducaoRepository(context);

        // ACT
        await repository.UpdateExistingAsync(fase);

        // ASSERT
        context.Fases_Producao.Single(f => f.Fases_producao_id == fase.Fases_producao_id)
            .Descricao.Should().Be("Descricao atualizada");
    }
}
