using TipMolde.Domain.Entities.Producao;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.Interface.Producao.IFasesProducao
{
    /// <summary>
    /// Define operacoes de persistencia especificas da feature FasesProducao.
    /// </summary>
    public interface IFasesProducaoRepository : IGenericRepository<FasesProducao, int>
    {
        /// <summary>
        /// Obtem uma fase pelo nome funcional.
        /// </summary>
        /// <param name="nome">Nome da fase a procurar.</param>
        /// <returns>Entidade encontrada; nulo caso nao exista.</returns>
        Task<FasesProducao?> GetByNomeAsync(Nome_fases nome);

        /// <summary>
        /// Verifica se a fase esta referenciada por maquinas.
        /// </summary>
        /// <param name="faseId">Identificador da fase.</param>
        /// <returns>True quando existir pelo menos uma maquina associada.</returns>
        Task<bool> HasMaquinasAssociadasAsync(int faseId);

        /// <summary>
        /// Persiste uma nova fase traduzindo conflitos tecnicos em conflitos de negocio.
        /// </summary>
        /// <param name="fase">Entidade a criar.</param>
        /// <returns>Entidade criada.</returns>
        Task<FasesProducao> CreateAsync(FasesProducao fase);

        /// <summary>
        /// Atualiza uma fase existente traduzindo conflitos tecnicos em conflitos de negocio.
        /// </summary>
        /// <param name="fase">Entidade a atualizar.</param>
        /// <returns>Task de conclusao da atualizacao.</returns>
        Task UpdateExistingAsync(FasesProducao fase);
    }
}
