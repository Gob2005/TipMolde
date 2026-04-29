using FluentAssertions;
using TipMolde.Domain.Entities.Desenho;
using TipMolde.Domain.Enums;
using TipMolde.Infrastructure.Repositorio;

namespace TipMolde.Tests.Integracao.Repositorio
{
    [TestFixture]
    [Category("Integration")]
    public sealed class ProjetoRepositoryTests : RepositoryIntegrationTestBase
    {
        [Test(Description = "TPROJREP1 - GetByMoldeId deve devolver projetos associados ao molde.")]
        public async Task GetByMoldeIdAsync_Should_ReturnProjetos_When_MoldeMatches()
        {
            // ARRANGE
            await using var context = CreateContext();
            await context.Projetos.AddRangeAsync(
                new Projeto { NomeProjeto = "Projeto B", SoftwareUtilizado = "SolidWorks", TipoProjeto = TipoProjeto.PROJETO_3D, CaminhoPastaServidor = "\\\\srv\\B", Molde_id = 1 },
                new Projeto { NomeProjeto = "Projeto A", SoftwareUtilizado = "SolidWorks", TipoProjeto = TipoProjeto.PROJETO_3D, CaminhoPastaServidor = "\\\\srv\\A", Molde_id = 1 },
                new Projeto { NomeProjeto = "Projeto C", SoftwareUtilizado = "AutoCAD", TipoProjeto = TipoProjeto.PROJETO_2D, CaminhoPastaServidor = "\\\\srv\\C", Molde_id = 2 });
            await context.SaveChangesAsync();

            var repository = new ProjetoRepository(context);

            // ACT
            var result = await repository.GetByMoldeIdAsync(1, page: 1, pageSize: 10);

            // ASSERT
            result.TotalCount.Should().Be(2);
            result.Items.Should().OnlyContain(p => p.Molde_id == 1);
        }

        [Test(Description = "TPROJREP2 - GetWithRevisoes deve carregar revisoes associadas ao projeto.")]
        public async Task GetWithRevisoesAsync_Should_LoadRevisoes_When_ProjetoExists()
        {
            // ARRANGE
            await using var context = CreateContext();
            var projeto = new Projeto
            {
                NomeProjeto = "Projeto A",
                SoftwareUtilizado = "SolidWorks",
                TipoProjeto = TipoProjeto.PROJETO_3D,
                CaminhoPastaServidor = "\\\\srv\\A",
                Molde_id = 1
            };
            projeto.Revisoes.Add(new Revisao { DescricaoAlteracoes = "Alteracao A", NumRevisao = 1, DataEnvioCliente = DateTime.UtcNow });

            await context.Projetos.AddAsync(projeto);
            await context.SaveChangesAsync();

            var repository = new ProjetoRepository(context);

            // ACT
            var result = await repository.GetWithRevisoesAsync(projeto.Projeto_id);

            // ASSERT
            result.Should().NotBeNull();
            result!.Revisoes.Should().ContainSingle(r => r.NumRevisao == 1);
        }
    }
}
