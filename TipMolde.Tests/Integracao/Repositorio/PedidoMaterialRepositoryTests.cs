using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;
using TipMolde.Infrastructure.Repositorio;

namespace TipMolde.Tests.Integracao.Repositorio;

[TestFixture]
[Category("Integration")]
public sealed class PedidoMaterialRepositoryTests : RepositoryIntegrationTestBase
{
    [Test(Description = "TPMREP1 - GetByIdWithItens deve carregar pedido com linhas associadas.")]
    public async Task GetByIdWithItensAsync_Should_LoadItens_When_PedidoExists()
    {
        // ARRANGE
        await using var context = CreateContext();

        var pedido = new PedidoMaterial
        {
            Fornecedor_id = 10,
            DataPedido = DateTime.UtcNow,
            Estado = EstadoPedido.PENDENTE,
            Itens =
            {
                new ItemPedidoMaterial { Peca_id = 100, Quantidade = 2 }
            }
        };

        await context.PedidosMaterial.AddAsync(pedido);
        await context.SaveChangesAsync();

        var repository = new PedidoMaterialRepository(context);

        // ACT
        var result = await repository.GetByIdWithItensAsync(pedido.PedidoMaterial_id);

        // ASSERT
        result.Should().NotBeNull();
        result!.Itens.Should().ContainSingle();
        result.Itens.Single().Peca_id.Should().Be(100);
    }

    [Test(Description = "TPMREP2 - GetPagedWithItens deve devolver pedidos ordenados por data e com linhas carregadas.")]
    public async Task GetPagedWithItensAsync_Should_ReturnOrderedPedidosWithItens()
    {
        // ARRANGE
        await using var context = CreateContext();
        await context.PedidosMaterial.AddRangeAsync(
            new PedidoMaterial
            {
                Fornecedor_id = 10,
                DataPedido = DateTime.UtcNow.AddDays(-2),
                Itens = { new ItemPedidoMaterial { Peca_id = 101, Quantidade = 1 } }
            },
            new PedidoMaterial
            {
                Fornecedor_id = 11,
                DataPedido = DateTime.UtcNow.AddDays(-1),
                Itens = { new ItemPedidoMaterial { Peca_id = 102, Quantidade = 2 } }
            });
        await context.SaveChangesAsync();

        var repository = new PedidoMaterialRepository(context);

        // ACT
        var result = await repository.GetPagedWithItensAsync(page: 1, pageSize: 10);

        // ASSERT
        result.TotalCount.Should().Be(2);
        result.Items.Select(p => p.Fornecedor_id).Should().ContainInOrder(11, 10);
        result.Items.Should().OnlyContain(p => p.Itens.Count == 1);
    }

    [Test(Description = "TPMREP3 - GetByFornecedorIdWithItens deve filtrar por fornecedor e paginar pedidos.")]
    public async Task GetByFornecedorIdWithItensAsync_Should_FilterByFornecedorAndPaginate()
    {
        // ARRANGE
        await using var context = CreateContext();
        await context.PedidosMaterial.AddRangeAsync(
            new PedidoMaterial { Fornecedor_id = 10, DataPedido = DateTime.UtcNow.AddDays(-3) },
            new PedidoMaterial { Fornecedor_id = 10, DataPedido = DateTime.UtcNow.AddDays(-2) },
            new PedidoMaterial { Fornecedor_id = 11, DataPedido = DateTime.UtcNow.AddDays(-1) });
        await context.SaveChangesAsync();

        var repository = new PedidoMaterialRepository(context);

        // ACT
        var result = await repository.GetByFornecedorIdWithItensAsync(fornecedorId: 10, page: 2, pageSize: 1);

        // ASSERT
        result.TotalCount.Should().Be(2);
        result.Items.Should().ContainSingle(p => p.Fornecedor_id == 10);
    }

    [Test(Description = "TPMREP4 - RegistarRececao deve atualizar pedido e pecas na mesma operacao.")]
    public async Task RegistarRececaoAsync_Should_UpdatePedidoAndPecas_When_DataIsValid()
    {
        // ARRANGE
        await using var context = CreateContext();

        var peca1 = new Peca { Designacao = "P1", Molde_id = 1 };
        var peca2 = new Peca { Designacao = "P2", Molde_id = 1 };

        await context.Pecas.AddRangeAsync(peca1, peca2);
        await context.SaveChangesAsync();

        var pedido = new PedidoMaterial
        {
            Fornecedor_id = 10,
            DataPedido = DateTime.UtcNow,
            Estado = EstadoPedido.PENDENTE
        };

        await context.PedidosMaterial.AddAsync(pedido);
        await context.SaveChangesAsync();

        await context.ItensPedidoMaterial.AddRangeAsync(
            new ItemPedidoMaterial { PedidoMaterial_id = pedido.PedidoMaterial_id, Peca_id = peca1.Peca_id, Quantidade = 1 },
            new ItemPedidoMaterial { PedidoMaterial_id = pedido.PedidoMaterial_id, Peca_id = peca2.Peca_id, Quantidade = 1 });
        await context.SaveChangesAsync();

        pedido.Estado = EstadoPedido.RECEBIDO;
        pedido.DataRececao = DateTime.UtcNow;
        pedido.UserConferente_id = 9;

        peca1.MaterialRecebido = true;
        peca2.MaterialRecebido = true;

        var repository = new PedidoMaterialRepository(context);

        // ACT
        await repository.RegistarRececaoAsync(pedido, new[] { peca1, peca2 });

        // ASSERT
        var persistedPedido = await context.PedidosMaterial.FindAsync(pedido.PedidoMaterial_id);
        var persistedPecas = await context.Pecas.OrderBy(p => p.Peca_id).ToListAsync();

        persistedPedido!.Estado.Should().Be(EstadoPedido.RECEBIDO);
        persistedPedido.UserConferente_id.Should().Be(9);
        persistedPecas.Should().OnlyContain(p => p.MaterialRecebido);
    }
}
