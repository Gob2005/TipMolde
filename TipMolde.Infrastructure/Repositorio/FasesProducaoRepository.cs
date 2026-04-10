using Microsoft.EntityFrameworkCore;
using TipMolde.Domain.Enums;
using TipMolde.Application.Interface.Producao.IFasesProducao;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class FasesProducaoRepository : GenericRepository<FasesProducao, int>, IFasesProducaoRepository
    {
        public FasesProducaoRepository(ApplicationDbContext context) : base(context)  { }

        public Task<FasesProducao?> GetByNomeAsync(Nome_fases nome)
        {
            return _context.Fases_Producao
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Nome == nome);
        }
    }
}
