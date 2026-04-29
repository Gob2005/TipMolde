using FluentAssertions;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;
using TipMolde.Infrastructure.Repositorio;

namespace TipMolde.Tests.Integracao.Repositorio
{
    [TestFixture]
    [Category("Integration")]
    public sealed class EncomendaMoldeRepositoryTests : RepositoryIntegrationTestBase
    {
        [Test(Description = "TENCMREP1 - GetByEncomendaId deve carregar molde associado.")]
        public async Task GetByEncomendaIdAsync_Should_LoadMolde_When_LinkExists()
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

            var repository = new EncomendaMoldeRepository(context);

            // ACT
            var result = await repository.GetByEncomendaIdAsync(encomenda.Encomenda_id, page: 1, pageSize: 10);

            // ASSERT
            var link = result.Items.Should().ContainSingle().Subject;
            link.Molde.Should().NotBeNull();
            link.Molde!.Numero.Should().Be("M-001");
        }

        [Test(Description = "TENCMREP2 - ExistsAssociation deve ignorar associacao excluida durante update.")]
        public async Task ExistsAssociationAsync_Should_IgnoreExcludedLink_When_UpdatingSameAssociation()
        {
            // ARRANGE
            await using var context = CreateContext();
            var link = new EncomendaMolde
            {
                Encomenda_id = 1,
                Molde_id = 2,
                Quantidade = 1,
                Prioridade = 1,
                DataEntregaPrevista = DateTime.UtcNow.AddDays(10)
            };

            await context.EncomendasMoldes.AddAsync(link);
            await context.SaveChangesAsync();

            var repository = new EncomendaMoldeRepository(context);

            // ACT
            var result = await repository.ExistsAssociationAsync(1, 2, link.EncomendaMolde_id);

            // ASSERT
            result.Should().BeFalse();
        }
    }

}
