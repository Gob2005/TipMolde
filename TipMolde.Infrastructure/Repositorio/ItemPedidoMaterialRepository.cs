using Microsoft.EntityFrameworkCore;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Comercio.IPedidoMaterial.IItemPedidoMaterial;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    /// <summary>
    /// Implementa operacoes de persistencia especificas para linhas de pedido de material.
    /// </summary>
    public class ItemPedidoMaterialRepository : GenericRepository<ItemPedidoMaterial, int>, IItemPedidoMaterialRepository
    {
        /// <summary>
        /// Construtor de ItemPedidoMaterialRepository.
        /// </summary>
        /// <param name="context">Contexto EF Core da aplicacao.</param>
        public ItemPedidoMaterialRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lista linhas de um pedido de material.
        /// </summary>
        /// <param name="pedidoId">Identificador do pedido.</param>
        /// <param name="page">Numero da pagina a consultar.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Resultado paginado com linhas associadas ao pedido informado.</returns>
        public async Task<PagedResult<ItemPedidoMaterial>> GetByPedidoIdAsync(int pedidoId, int page = 1, int pageSize = 10)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize > 200 ? 200 : pageSize;

            var query = _context.ItensPedidoMaterial
                .Include(i => i.Peca)
                .Where(i => i.PedidoMaterial_id == pedidoId);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderBy(i => i.ItemPedidoMaterial_id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ItemPedidoMaterial>(items, totalCount, page, pageSize);
        }
    }
}
