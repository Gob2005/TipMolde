using Microsoft.EntityFrameworkCore;
using TipMolde.Core.Interface.Comercio.IPedidoMaterial.IItemPedidoMaterial;
using TipMolde.Core.Models.Comercio;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class ItemPedidoMaterialRepository : GenericRepository<ItemPedidoMaterial>, IItemPedidoMaterialRepository
    {
        public ItemPedidoMaterialRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<ItemPedidoMaterial>> GetByPedidoIdAsync(int pedidoId)
        {
            return await _context.ItensPedidoMaterial
                .Include(i => i.Peca)
                .Where(i => i.PedidoMaterial_id == pedidoId)
                .ToListAsync();
        }
    }
}
