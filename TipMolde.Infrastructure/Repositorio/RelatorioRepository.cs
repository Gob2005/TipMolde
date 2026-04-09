using Microsoft.EntityFrameworkCore;
using TipMolde.Core.Interface.Relatorios;
using TipMolde.Core.Models.Fichas;
using TipMolde.Core.Models.Producao;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class RelatorioRepository : IRelatorioRepository
    {
        private readonly ApplicationDbContext _context;

        public RelatorioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<Molde?> GetMoldeComEspecificacoesAsync(int moldeId) =>
            _context.Moldes
                .Include(m => m.Especificacoes)
                .FirstOrDefaultAsync(m => m.Molde_id == moldeId);

        public Task<FichaProducao?> GetFichaFltCompletaAsync(int fichaId) =>
            _context.FichasProducao
                .Include(f => f.EncomendaMolde)
                    .ThenInclude(em => em.Encomenda)
                        .ThenInclude(e => e.Cliente)
                .Include(f => f.EncomendaMolde)
                    .ThenInclude(em => em.Molde)
                        .ThenInclude(m => m.Especificacoes)
                .FirstOrDefaultAsync(f => f.FichaProducao_id == fichaId);
    }
}
