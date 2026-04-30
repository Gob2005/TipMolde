using Microsoft.EntityFrameworkCore;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Fichas.IFichaDocumento;
using TipMolde.Domain.Entities.Fichas;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    /// <summary>
    /// Implementa a persistencia e consulta de metadados documentais das fichas de producao.
    /// </summary>
    public class FichaDocumentoRepository : IFichaDocumentoRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor de FichaDocumentoRepository.
        /// </summary>
        /// <param name="context">Contexto EF Core usado para aceder as tabelas documentais das fichas.</param>
        public FichaDocumentoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Verifica se a ficha de producao existe.
        /// </summary>
        /// <param name="fichaId">Identificador da ficha.</param>
        /// <returns>True quando a ficha existe.</returns>
        public Task<bool> FichaExisteAsync(int fichaId) =>
            _context.FichasProducao.AnyAsync(f => f.FichaProducao_id == fichaId);

        /// <summary>
        /// Calcula a proxima versao documental da ficha.
        /// </summary>
        /// <param name="fichaId">Identificador da ficha.</param>
        /// <returns>Numero sequencial da proxima versao.</returns>
        public async Task<int> GetProximaVersaoAsync(int fichaId)
        {
            var max = await _context.FichasDocumentos
                .Where(x => x.FichaProducao_id == fichaId)
                .Select(x => (int?)x.Versao)
                .MaxAsync();

            return (max ?? 0) + 1;
        }

        /// <summary>
        /// Desativa todas as versoes atualmente ativas da ficha.
        /// </summary>
        /// <remarks>
        /// Esta operacao deve correr dentro da mesma transacao que vai persistir a nova versao.
        /// </remarks>
        /// <param name="fichaId">Identificador da ficha.</param>
        public async Task DesativarVersoesAtivasAsync(int fichaId)
        {
            var ativos = await _context.FichasDocumentos
                .Where(x => x.FichaProducao_id == fichaId && x.Ativo)
                .ToListAsync();

            foreach (var documentoAtivo in ativos)
                documentoAtivo.Ativo = false;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Persiste um novo documento da ficha.
        /// </summary>
        /// <param name="doc">Entidade documental a persistir.</param>
        public async Task AddAsync(FichaDocumento doc)
        {
            await _context.FichasDocumentos.AddAsync(doc);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtem um documento pelo identificador interno.
        /// </summary>
        /// <param name="id">Identificador do documento.</param>
        /// <returns>Documento encontrado ou nulo.</returns>
        public Task<FichaDocumento?> GetByIdAsync(int id) =>
            _context.FichasDocumentos.FirstOrDefaultAsync(x => x.FichaDocumento_id == id);

        /// <summary>
        /// Obtem um documento garantindo que pertence a uma ficha especifica.
        /// </summary>
        /// <param name="fichaId">Identificador da ficha.</param>
        /// <param name="documentoId">Identificador do documento.</param>
        /// <returns>Documento encontrado ou nulo.</returns>
        public Task<FichaDocumento?> GetByIdAndFichaIdAsync(int fichaId, int documentoId) =>
            _context.FichasDocumentos.FirstOrDefaultAsync(x =>
                x.FichaProducao_id == fichaId &&
                x.FichaDocumento_id == documentoId);

        /// <summary>
        /// Obtem a versao ativa mais recente de uma ficha.
        /// </summary>
        /// <param name="fichaId">Identificador da ficha.</param>
        /// <returns>Documento ativo ou nulo.</returns>
        public Task<FichaDocumento?> GetAtivoByFichaIdAsync(int fichaId) =>
            _context.FichasDocumentos
                .Where(x => x.FichaProducao_id == fichaId && x.Ativo)
                .OrderByDescending(x => x.Versao)
                .FirstOrDefaultAsync();

        /// <summary>
        /// Lista todas as versoes documentais de uma ficha.
        /// </summary>
        /// <param name="fichaId">Identificador da ficha.</param>
        /// <returns>Colecao ordenada por versao decrescente.</returns>
        public async Task<PagedResult<FichaDocumento>> GetByFichaIdAsync(int fichaId, int page, int pageSize)
        {
            var query = _context.FichasDocumentos
                .AsNoTracking()
                .Where(x => x.FichaProducao_id == fichaId)
                .OrderByDescending(x => x.Versao);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<FichaDocumento>(items, totalCount, page, pageSize);
        }
    }
}
