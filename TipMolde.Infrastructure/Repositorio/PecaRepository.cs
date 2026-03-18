using Microsoft.EntityFrameworkCore;
using TipMolde.Core.Interface.IPeca;
using TipMolde.Core.Models;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class PecaRepository : GenericRepository<Peca>, IPecaRepository
    {
        public PecaRepository(ApplicationDbContext context) : base(context) { }

        public Task<Peca?> GetByNumberAsync(int numero)
        {
            return _context.Pecas.FirstOrDefaultAsync(p => p.Numero_peca == numero);
        }
    }
}
