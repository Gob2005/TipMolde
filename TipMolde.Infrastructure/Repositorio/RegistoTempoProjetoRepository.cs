using System.Data;
using Microsoft.EntityFrameworkCore;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Desenho.IRegistoTempoProjeto;
using TipMolde.Domain.Entities.Desenho;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    /// <summary>
    /// Implementa operacoes de persistencia especificas para RegistoTempoProjeto.
    /// </summary>
    public class RegistoTempoProjetoRepository : GenericRepository<RegistoTempoProjeto, int>, IRegistoTempoProjetoRepository
    {
        /// <summary>
        /// Construtor de RegistoTempoProjetoRepository.
        /// </summary>
        /// <param name="context">Contexto EF Core da aplicacao.</param>
        public RegistoTempoProjetoRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lista o historico temporal de um projeto para um autor.
        /// </summary>
        /// <param name="projetoId">Identificador do projeto.</param>
        /// <param name="autorId">Identificador do autor.</param>
        /// <param name="page">Numero da pagina a ser retornada.</param>
        /// <param name="pageSize">Tamanho da pagina.</param>
        /// <returns>Colecao ordenada por data e identificador.</returns>
        public async Task<PagedResult<RegistoTempoProjeto>> GetHistoricoAsync(int projetoId, int autorId, int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize > 200 ? 200 : pageSize;

            var query = _context.RegistosTempoProjeto
                .AsNoTracking()
                .Where(r => r.Projeto_id == projetoId && r.Autor_id == autorId);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(r => r.Data_hora)
                .ThenBy(r => r.Registo_Tempo_Projeto_id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<RegistoTempoProjeto>(items, totalCount, page, pageSize);
        }

        /// <summary>
        /// Obtem o ultimo registo temporal de um projeto para um autor.
        /// </summary>
        /// <param name="projetoId">Identificador do projeto.</param>
        /// <param name="autorId">Identificador do autor.</param>
        /// <returns>Ultimo registo encontrado; nulo quando nao existe historico.</returns>
        public Task<RegistoTempoProjeto?> GetUltimoRegistoAsync(int projetoId, int autorId)
        {
            return _context.RegistosTempoProjeto
                .AsNoTracking()
                .Where(r => r.Projeto_id == projetoId && r.Autor_id == autorId)
                .OrderByDescending(r => r.Data_hora)
                .ThenByDescending(r => r.Registo_Tempo_Projeto_id)
                .FirstOrDefaultAsync();
        }
    }
}
