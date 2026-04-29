using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TipMolde.Domain.Entities.Desenho;
using TipMolde.Infrastructure.Repositorio;

namespace TipMolde.Tests.Integracao.Repositorio
{
    [TestFixture]
    [Category("Integration")]
    public sealed class RevisaoRepositoryTests : RepositoryIntegrationTestBase
    {
        [Test(Description = "TREVREP1 - AddWithGeneratedNumero deve atribuir proximo numero sequencial por projeto.")]
        public async Task AddWithGeneratedNumeroAsync_Should_AssignNextNumero_When_ProjectHasRevisions()
        {
            // ARRANGE
            await using var context = CreateContext();
            await context.Revisoes.AddAsync(new Revisao
            {
                Projeto_id = 2,
                NumRevisao = 3,
                DescricaoAlteracoes = "Revisao existente",
                DataEnvioCliente = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            var repository = new RevisaoRepository(context);

            // ACT
            var result = await repository.AddWithGeneratedNumeroAsync(new Revisao
            {
                Projeto_id = 2,
                DescricaoAlteracoes = "Nova revisao",
                DataEnvioCliente = DateTime.UtcNow
            });

            // ASSERT
            result.NumRevisao.Should().Be(4);
            (await context.Revisoes.CountAsync()).Should().Be(2);
        }

        [Test(Description = "TREVREP2 - GetByProjetoId deve devolver revisoes ordenadas por numero decrescente.")]
        public async Task GetByProjetoIdAsync_Should_ReturnDescendingRevisoes_When_ProjectMatches()
        {
            // ARRANGE
            await using var context = CreateContext();
            await context.Revisoes.AddRangeAsync(
                new Revisao { Projeto_id = 2, NumRevisao = 1, DescricaoAlteracoes = "R1", DataEnvioCliente = DateTime.UtcNow },
                new Revisao { Projeto_id = 2, NumRevisao = 2, DescricaoAlteracoes = "R2", DataEnvioCliente = DateTime.UtcNow });
            await context.SaveChangesAsync();

            var repository = new RevisaoRepository(context);

            // ACT
            var result = await repository.GetByProjetoIdAsync(2, page: 1, pageSize: 10);

            // ASSERT
            result.Items.Select(r => r.NumRevisao).Should().ContainInOrder(2, 1);
        }
    }
}
