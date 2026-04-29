using FluentAssertions;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;
using TipMolde.Infrastructure.Repositorio;

namespace TipMolde.Tests.Integracao.Repositorio
{
    [TestFixture]
    [Category("Integration")]
    public sealed class MoldeRepositoryTests : RepositoryIntegrationTestBase
    {
        [Test(Description = "TMOLREP1 - GetByNumero deve carregar especificacoes tecnicas do molde.")]
        public async Task GetByNumeroAsync_Should_LoadEspecificacoes_When_MoldeExists()
        {
            // ARRANGE
            await using var context = CreateContext();
            var molde = new Molde { Numero = "M-001", Numero_cavidades = 2, TipoPedido = TipoPedido.NOVO_MOLDE };
            await context.Moldes.AddAsync(molde);
            await context.SaveChangesAsync();

            await context.EspecificacoesTecnicas.AddAsync(new EspecificacoesTecnicas
            {
                Molde_id = molde.Molde_id,
                MaterialMacho = "AISI P20"
            });
            await context.SaveChangesAsync();

            var repository = new MoldeRepository(context);

            // ACT
            var result = await repository.GetByNumeroAsync("M-001");

            // ASSERT
            result.Should().NotBeNull();
            result!.Especificacoes.Should().NotBeNull();
            result.Especificacoes!.MaterialMacho.Should().Be("AISI P20");
        }

        [Test(Description = "TMOLREP2 - GetByEncomendaId deve devolver moldes associados a encomenda.")]
        public async Task GetByEncomendaIdAsync_Should_ReturnMoldes_When_LinkExists()
        {
            // ARRANGE
            await using var context = CreateContext();
            var encomenda = new Encomenda { NumeroEncomendaCliente = "ENC-001" };
            var molde = new Molde { Numero = "M-001", Numero_cavidades = 2, TipoPedido = TipoPedido.NOVO_MOLDE };

            await context.Encomendas.AddAsync(encomenda);
            await context.Moldes.AddAsync(molde);
            await context.SaveChangesAsync();

            await context.EncomendasMoldes.AddAsync(new EncomendaMolde
            {
                Encomenda_id = encomenda.Encomenda_id,
                Molde_id = molde.Molde_id,
                Quantidade = 1,
                Prioridade = 1,
                DataEntregaPrevista = DateTime.UtcNow.AddDays(10)
            });
            await context.SaveChangesAsync();

            var repository = new MoldeRepository(context);

            // ACT
            var result = await repository.GetByEncomendaIdAsync(encomenda.Encomenda_id, page: 1, pageSize: 10);

            // ASSERT
            result.Items.Should().ContainSingle(m => m.Numero == "M-001");
        }
    }
}
