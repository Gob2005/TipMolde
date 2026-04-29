using FluentAssertions;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Infrastructure.Repositorio;

namespace TipMolde.Tests.Integracao.Repositorio;

[TestFixture]
[Category("Integration")]
public sealed class ItemPedidoMaterialRepositoryTests : RepositoryIntegrationTestBase
{
    [Test(Description = "TIPMREP1 - GetByPedidoId deve filtrar linhas por pedido, incluir peca e paginar por ID.")]
    public async Task GetByPedidoIdAsync_Should_FilterIncludePecaAndPaginate()
    {
        // ARRANGE
        await using var context = CreateContext();
        var pedido1 = new PedidoMaterial { Fornecedor_id = 1, DataPedido = DateTime.UtcNow };
        var pedido2 = new PedidoMaterial { Fornecedor_id = 2, DataPedido = DateTime.UtcNow };
        var peca1 = new Peca { Designacao = "Placa", Molde_id = 1 };
        var peca2 = new Peca { Designacao = "Extrator", Molde_id = 1 };
        var peca3 = new Peca { Designacao = "Postico", Molde_id = 1 };

        await context.PedidosMaterial.AddRangeAsync(pedido1, pedido2);
        await context.Pecas.AddRangeAsync(peca1, peca2, peca3);
        await context.SaveChangesAsync();

        await context.ItensPedidoMaterial.AddRangeAsync(
            new ItemPedidoMaterial { ItemPedidoMaterial_id = 10, PedidoMaterial_id = pedido1.PedidoMaterial_id, Peca_id = peca1.Peca_id, Quantidade = 1 },
            new ItemPedidoMaterial { ItemPedidoMaterial_id = 20, PedidoMaterial_id = pedido1.PedidoMaterial_id, Peca_id = peca2.Peca_id, Quantidade = 2 },
            new ItemPedidoMaterial { ItemPedidoMaterial_id = 30, PedidoMaterial_id = pedido2.PedidoMaterial_id, Peca_id = peca3.Peca_id, Quantidade = 3 });
        await context.SaveChangesAsync();

        var repository = new ItemPedidoMaterialRepository(context);

        // ACT
        var result = await repository.GetByPedidoIdAsync(pedido1.PedidoMaterial_id, page: 2, pageSize: 1);

        // ASSERT
        result.TotalCount.Should().Be(2);
        result.Items.Should().ContainSingle();
        result.Items.Single().Peca.Should().NotBeNull();
        result.Items.Single().Peca!.Designacao.Should().Be("Extrator");
    }
}
