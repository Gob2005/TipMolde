using Microsoft.EntityFrameworkCore;
using TipMolde.Core.Interface.IPeca;
using TipMolde.Core.Models;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class PecaRepository : GenericRepository<Peca>, IPecaRepository
    {
        public PecaRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Peca>> GetByMoldeIdAsync(int moldeId)
        {
            return await _context.Pecas
                .Where(p => p.Molde_id == moldeId)
                .ToListAsync();
        }

        public Task<Peca?> GetByDesignacaoAsync(string designacao, int moldeId)
        {
            return _context.Pecas
                .FirstOrDefaultAsync(p => p.Designacao == designacao && p.Molde_id == moldeId);
        }
    }
}
