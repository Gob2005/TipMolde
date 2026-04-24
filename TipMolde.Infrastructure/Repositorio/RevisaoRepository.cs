using Microsoft.EntityFrameworkCore;
using System.Data;
using TipMolde.Application.Exceptions;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Desenho.IRevisao;
using TipMolde.Domain.Entities.Desenho;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    /// <summary>
    /// Implementa a persistencia da feature Revisao.
    /// </summary>
    public class RevisaoRepository : GenericRepository<Revisao, int>, IRevisaoRepository
    {
        /// <summary>
        /// Construtor de RevisaoRepository.
        /// </summary>
        /// <param name="context">Contexto EF Core da aplicacao.</param>
        public RevisaoRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Lista revisoes associadas a um projeto.
        /// </summary>
        /// <param name="projetoId">Identificador do projeto.</param>
        /// <param name="page">Numero da pagina a ser retornada.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Colecao de revisoes ordenadas por numero decrescente.</returns>
        public async Task<PagedResult<Revisao>> GetByProjetoIdAsync(int projetoId, int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize > 200 ? 200 : pageSize;

            var query = _context.Revisoes
                .AsNoTracking()
                .Where(r => r.Projeto_id == projetoId);

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(r => r.NumRevisao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Revisao>(items, totalCount, page, pageSize);
        }

        /// <summary>
        /// Persiste uma nova revisao atribuindo o numero de forma automatica.
        /// </summary>
        /// <param name="revisao">Entidade de revisao a persistir.</param>
        /// <returns>Entidade persistida com `Revisao_id` e `NumRevisao` preenchidos.</returns>
        public async Task<Revisao> AddWithGeneratedNumeroAsync(Revisao revisao)
        {
            var max = await _context.Revisoes
                .Where(r => r.Projeto_id == revisao.Projeto_id)
                .Select(r => (int?)r.NumRevisao)
                .MaxAsync();

            revisao.NumRevisao = (max ?? 0) + 1;

            await _context.Revisoes.AddAsync(revisao);
            await _context.SaveChangesAsync();

            return revisao;
        }
    }
}
