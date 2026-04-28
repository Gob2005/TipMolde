using Microsoft.EntityFrameworkCore;
using TipMolde.Application.Exceptions;
using TipMolde.Application.Interface.Producao.IFasesProducao;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    /// <summary>
    /// Implementa o acesso a dados da feature FasesProducao.
    /// </summary>
    /// <remarks>
    /// Traduz conflitos tecnicos de unicidade em conflitos de negocio
    /// para estabilizar o contrato observado pela camada de aplicacao.
    /// </remarks>
    public class FasesProducaoRepository : GenericRepository<FasesProducao, int>, IFasesProducaoRepository
    {
        /// <summary>
        /// Construtor de FasesProducaoRepository.
        /// </summary>
        /// <param name="context">Contexto EF da aplicacao.</param>
        public FasesProducaoRepository(ApplicationDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Obtem uma fase pelo nome funcional.
        /// </summary>
        /// <param name="nome">Nome da fase a procurar.</param>
        /// <returns>Entidade encontrada; nulo caso nao exista.</returns>
        public Task<FasesProducao?> GetByNomeAsync(NomeFases nome)
        {
            return _context.Fases_Producao
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Nome == nome);
        }

        /// <summary>
        /// Verifica se a fase esta referenciada por maquinas.
        /// </summary>
        /// <param name="faseId">Identificador da fase.</param>
        /// <returns>True quando existir pelo menos uma maquina associada.</returns>
        public Task<bool> HasMaquinasAssociadasAsync(int faseId)
        {
            return _context.Maquinas
                .AsNoTracking()
                .AnyAsync(m => m.FaseDedicada_id == faseId);
        }

        /// <summary>
        /// Persiste uma nova fase e traduz conflito de indice unico para conflito de negocio.
        /// </summary>
        /// <param name="fase">Entidade a criar.</param>
        /// <returns>Entidade criada.</returns>
        public async Task<FasesProducao> CreateAsync(FasesProducao fase)
        {
            try
            {
                await _context.Fases_Producao.AddAsync(fase);
                await _context.SaveChangesAsync();
                return fase;
            }
            catch (DbUpdateException ex) when (IsUniqueNomeViolation(ex))
            {
                throw new BusinessConflictException("Ja existe uma fase de producao com esse nome.");
            }
        }

        /// <summary>
        /// Atualiza uma fase existente e traduz conflito de indice unico para conflito de negocio.
        /// </summary>
        /// <param name="fase">Entidade a atualizar.</param>
        /// <returns>Task de conclusao da atualizacao.</returns>
        public async Task UpdateExistingAsync(FasesProducao fase)
        {
            try
            {
                _context.Fases_Producao.Update(fase);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (IsUniqueNomeViolation(ex))
            {
                throw new BusinessConflictException("Ja existe uma fase de producao com esse nome.");
            }
        }

        /// <summary>
        /// Avalia se a excecao recebida corresponde a violacao do indice unico de nome.
        /// </summary>
        /// <param name="ex">Excecao original do Entity Framework.</param>
        /// <returns>True quando a excecao representar duplicado funcional no nome.</returns>
        private static bool IsUniqueNomeViolation(DbUpdateException ex)
        {
            var message = ex.InnerException?.Message ?? ex.Message;

            return message.Contains("Duplicate entry", StringComparison.OrdinalIgnoreCase)
                || message.Contains("unique", StringComparison.OrdinalIgnoreCase);
        }
    }
}
