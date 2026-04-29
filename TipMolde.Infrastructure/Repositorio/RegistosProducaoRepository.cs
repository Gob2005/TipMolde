using Microsoft.EntityFrameworkCore;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Producao.IRegistosProducao;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    /// <summary>
    /// Implementa o acesso a dados da feature RegistosProducao.
    /// </summary>
    /// <remarks>
    /// Centraliza consultas historicas e a persistencia atomica entre registo de producao
    /// e estado da maquina, preservando rastreabilidade operacional.
    /// </remarks>
    public class RegistosProducaoRepository : GenericRepository<RegistosProducao, int>, IRegistosProducaoRepository
    {
        /// <summary>
        /// Construtor de RegistosProducaoRepository.
        /// </summary>
        /// <param name="context">Contexto EF Core da aplicacao.</param>
        public RegistosProducaoRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Obtem o historico de registos de uma peca numa fase.
        /// </summary>
        /// <param name="faseId">Identificador da fase de producao.</param>
        /// <param name="pecaId">Identificador da peca.</param>
        /// <param name="page">Pagina atual.</param>
        /// <param name="pageSize">Tamanho da pagina.</param>
        /// <returns>Resultado paginado com registos em ordem cronologica.</returns>
        public async Task<PagedResult<RegistosProducao>> GetHistoricoAsync(int faseId, int pecaId, int page, int pageSize)
        {
            var query = _context.RegistosProducao
                .AsNoTracking()
                .Where(r => r.Fase_id == faseId && r.Peca_id == pecaId)
                .OrderBy(r => r.Data_hora);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<RegistosProducao>(items, totalCount, page, pageSize);
        }

        /// <summary>
        /// Obtem o ultimo registo de uma peca numa fase.
        /// </summary>
        /// <param name="faseId">Identificador da fase de producao.</param>
        /// <param name="pecaId">Identificador da peca.</param>
        /// <returns>Ultimo registo encontrado ou nulo quando nao existe historico.</returns>
        public Task<RegistosProducao?> GetUltimoRegistoAsync(int faseId, int pecaId)
        {
            return _context.RegistosProducao
                .AsNoTracking()
                .Where(r => r.Fase_id == faseId && r.Peca_id == pecaId)
                .OrderByDescending(r => r.Data_hora)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Lista registos associados a uma maquina.
        /// </summary>
        /// <param name="maquinaId">Identificador da maquina.</param>
        /// <param name="page">Pagina atual.</param>
        /// <param name="pageSize">Tamanho da pagina.</param>
        /// <returns>Resultado paginado com registos associados a maquina.</returns>
        public async Task<PagedResult<RegistosProducao>> GetByMaquinaAsync(int maquinaId, int page, int pageSize)
        {
            var query = _context.RegistosProducao
                .AsNoTracking()
                .Where(r => r.Maquina_id == maquinaId)
                .OrderByDescending(r => r.Data_hora);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<RegistosProducao>(items, totalCount, page, pageSize);
        }

        /// <summary>
        /// Persiste um registo e a alteracao de estado da maquina na mesma transacao.
        /// </summary>
        /// <remarks>
        /// Porque: a maquina nao pode ficar ocupada ou disponivel sem o registo
        /// correspondente que justifica a transicao operacional.
        /// </remarks>
        /// <param name="registo">Registo de producao a criar.</param>
        /// <param name="maquinaToUpdate">Maquina a atualizar; nulo quando nao ha alteracao de maquina.</param>
        /// <returns>Registo persistido.</returns>
        public async Task<RegistosProducao> AddWithMachineStateAsync(RegistosProducao registo, Maquina? maquinaToUpdate)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (maquinaToUpdate != null)
                    _context.Maquinas.Update(maquinaToUpdate);

                await _context.RegistosProducao.AddAsync(registo);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return registo;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
