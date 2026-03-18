using Microsoft.EntityFrameworkCore;
using TipMolde.Core.Enums;
using TipMolde.Core.Interface.IFases_producao;
using TipMolde.Core.Models;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class FasesProducaoRepository : GenericRepository<FasesProducao>, IFasesProducaoRepository
    {
        public FasesProducaoRepository(ApplicationDbContext context) : base(context)  { }

        public Task<FasesProducao?> GetByNomeAsync(Nome_fases nome)
        {
            return _context.Set<FasesProducao>()
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Nome == nome);
        }
    }
}
