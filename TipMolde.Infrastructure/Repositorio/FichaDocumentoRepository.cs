using Microsoft.EntityFrameworkCore;
using TipMolde.Core.Interface.Fichas.IFichaDocumento;
using TipMolde.Core.Models.Fichas;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class FichaDocumentoRepository : IFichaDocumentoRepository
    {
        private readonly ApplicationDbContext _context;

        public FichaDocumentoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<bool> FichaExisteAsync(int fichaId) =>
            _context.FichasProducao.AnyAsync(f => f.FichaProducao_id == fichaId);

        public async Task<int> GetProximaVersaoAsync(int fichaId)
        {
            var max = await _context.FichasDocumentos
                .Where(x => x.FichaProducao_id == fichaId)
                .Select(x => (int?)x.Versao)
                .MaxAsync();

            return (max ?? 0) + 1;
        }

        public async Task DesativarVersoesAtivasAsync(int fichaId)
        {
            var ativos = await _context.FichasDocumentos
                .Where(x => x.FichaProducao_id == fichaId && x.Ativo)
                .ToListAsync();

            foreach (var a in ativos) a.Ativo = false;
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(FichaDocumento doc)
        {
            await _context.FichasDocumentos.AddAsync(doc);
            await _context.SaveChangesAsync();
        }

        public Task<FichaDocumento?> GetByIdAsync(int id) =>
            _context.FichasDocumentos.FirstOrDefaultAsync(x => x.FichaDocumento_id == id);

        public Task<FichaDocumento?> GetAtivoByFichaIdAsync(int fichaId) =>
            _context.FichasDocumentos
                .Where(x => x.FichaProducao_id == fichaId && x.Ativo)
                .OrderByDescending(x => x.Versao)
                .FirstOrDefaultAsync();

        public async Task<IEnumerable<FichaDocumento>> GetByFichaIdAsync(int fichaId) =>
            await _context.FichasDocumentos
                .Where(x => x.FichaProducao_id == fichaId)
                .OrderByDescending(x => x.Versao)
                .ToListAsync();
    }
}
