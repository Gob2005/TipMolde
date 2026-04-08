using Microsoft.EntityFrameworkCore;
using TipMolde.Core.Interface.Comercio.IPedidoMaterial;
using TipMolde.Core.Models.Comercio;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class PedidoMaterialRepository : GenericRepository<PedidoMaterial>, IPedidoMaterialRepository
    {
        public PedidoMaterialRepository(ApplicationDbContext context) : base(context) { }

        public Task<PedidoMaterial?> GetWithItensAsync(int id) =>
            _context.PedidosMaterial
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Peca)
                .FirstOrDefaultAsync(p => p.PedidoMaterial_id == id);

        public async Task<IEnumerable<PedidoMaterial>> GetByFornecedorIdAsync(int fornecedorId)
        {
            return await _context.PedidosMaterial
                .Where(p => p.Fornecedor_id == fornecedorId)
                .ToListAsync();
        }
    }
}
