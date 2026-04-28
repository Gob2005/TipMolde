using Microsoft.EntityFrameworkCore;
using TipMolde.Application.Exceptions;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Producao.IMaquina;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    /// <summary>
    /// Implementa o acesso a dados da feature Maquina.
    /// </summary>
    /// <remarks>
    /// Traduz conflitos tecnicos de unicidade em conflitos de negocio
    /// e centraliza consultas especificas do agregado.
    /// </remarks>
    public class MaquinaRepository : GenericRepository<Maquina, int>, IMaquinaRepository
    {
        /// <summary>
        /// Construtor de MaquinaRepository.
        /// </summary>
        /// <param name="context">Contexto EF da aplicacao.</param>
        public MaquinaRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Obtem uma maquina pelo identificador interno.
        /// </summary>
        /// <param name="id">Identificador da maquina.</param>
        /// <returns>Entidade encontrada; nulo caso nao exista.</returns>
        public Task<Maquina?> GetByIdUnicoAsync(int id)
        {
            return _context.Maquinas.FirstOrDefaultAsync(m => m.Maquina_id == id);
        }

        /// <summary>
        /// Lista maquinas por estado com paginacao.
        /// </summary>
        /// <param name="estado">Estado operacional a filtrar.</param>
        /// <param name="page">Pagina atual.</param>
        /// <param name="pageSize">Tamanho da pagina.</param>
        /// <returns>Resultado paginado com maquinas filtradas.</returns>
        public async Task<PagedResult<Maquina>> GetByEstadoAsync(EstadoMaquina estado, int page, int pageSize)
        {

            var query = _context.Maquinas
                .AsNoTracking()
                .Where(m => m.Estado == estado)
                .OrderBy(m => m.Numero);

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Maquina>(items, totalCount, page, pageSize);
        }

        /// <summary>
        /// Verifica se ja existe uma maquina com o numero fisico informado.
        /// </summary>
        /// <param name="numero">Numero fisico a validar.</param>
        /// <param name="excludeMaquinaId">Identificador a excluir na validacao, usado em updates.</param>
        /// <returns>True quando o numero ja estiver em uso.</returns>
        public Task<bool> ExistsNumeroAsync(int numero, int? excludeMaquinaId = null)
        {
            return _context.Maquinas
                .AsNoTracking()
                .AnyAsync(m =>
                    m.Numero == numero &&
                    (!excludeMaquinaId.HasValue || m.Maquina_id != excludeMaquinaId.Value));
        }

        /// <summary>
        /// Verifica se a fase dedicada existe.
        /// </summary>
        /// <param name="faseDedicadaId">Identificador da fase a validar.</param>
        /// <returns>True quando a fase existir.</returns>
        public Task<bool> ExistsFaseDedicadaAsync(int faseDedicadaId)
        {
            return _context.Fases_Producao
                .AsNoTracking()
                .AnyAsync(f => f.Fases_producao_id == faseDedicadaId);
        }

        /// <summary>
        /// Persiste uma nova maquina e traduz conflito de indice unico para conflito de negocio.
        /// </summary>
        /// <param name="maquina">Entidade a criar.</param>
        /// <returns>Entidade criada.</returns>
        public async Task<Maquina> CreateAsync(Maquina maquina)
        {
            try
            {
                await _context.Maquinas.AddAsync(maquina);
                await _context.SaveChangesAsync();
                return maquina;
            }
            catch (DbUpdateException ex) when (IsUniqueNumeroViolation(ex))
            {
                throw new BusinessConflictException($"Ja existe uma maquina com o numero '{maquina.Numero}'.");
            }
        }

        /// <summary>
        /// Atualiza uma maquina existente e traduz conflito de indice unico para conflito de negocio.
        /// </summary>
        /// <param name="maquina">Entidade a atualizar.</param>
        /// <returns>Task de conclusao da atualizacao.</returns>
        public async Task UpdateExistingAsync(Maquina maquina)
        {
            try
            {
                _context.Maquinas.Update(maquina);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (IsUniqueNumeroViolation(ex))
            {
                throw new BusinessConflictException($"Ja existe uma maquina com o numero '{maquina.Numero}'.");
            }
        }

        /// <summary>
        /// Avalia se a excecao recebida corresponde a violacao do indice unico de numero.
        /// </summary>
        /// <param name="ex">Excecao original do Entity Framework.</param>
        /// <returns>True quando a excecao representar duplicado funcional no numero.</returns>
        private static bool IsUniqueNumeroViolation(DbUpdateException ex)
        {
            var message = ex.InnerException?.Message ?? ex.Message;

            return message.Contains("Duplicate entry", StringComparison.OrdinalIgnoreCase)
                || (message.Contains("unique", StringComparison.OrdinalIgnoreCase)
                    && message.Contains("Numero", StringComparison.OrdinalIgnoreCase));
        }
    }
}
