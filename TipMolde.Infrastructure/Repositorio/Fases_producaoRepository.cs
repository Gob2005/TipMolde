using Microsoft.EntityFrameworkCore;
using TipMolde.Core.Enums;
using TipMolde.Core.Interface.IFases_producao;
using TipMolde.Core.Models;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class Fases_producaoRepository : GenericRepository<Fases_producao>, IFases_producaoRepository
    {
        public Fases_producaoRepository(ApplicationDbContext context) : base(context)  { }

        public Task<Fases_producao?> GetByNomeAsync(Nome_fases nome)
        {
            return _context.Set<Fases_producao>()
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Nome == nome);
        }
    }
}
