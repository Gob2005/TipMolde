using Microsoft.EntityFrameworkCore;
using TipMolde.Application.Interface.Comercio.IPedidoMaterial.IItemPedidoMaterial;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class ItemPedidoMaterialRepository : GenericRepository<ItemPedidoMaterial, int>, IItemPedidoMaterialRepository
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
