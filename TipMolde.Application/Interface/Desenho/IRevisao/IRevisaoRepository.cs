using TipMolde.Domain.Entities.Desenho;

namespace TipMolde.Application.Interface.Desenho.IRevisao
{
    /// <summary>
    /// Define as operacoes de persistencia da feature Revisao.
    /// </summary>
    public interface IRevisaoRepository : IGenericRepository<Revisao, int>
    {
        /// <summary>
        /// Lista revisoes associadas a um projeto.
        /// </summary>
        /// <param name="projetoId">Identificador do projeto.</param>
        /// <param name="page">Numero da pagina a ser retornada.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Colecao de revisoes ordenadas por numero decrescente.</returns>
        Task<PagedResult<Revisao>> GetByProjetoIdAsync(int projetoId, int page, int pageSize);

        /// <summary>
        /// Persiste uma nova revisao atribuindo o proximo numero de forma consistente.
        /// </summary>
        /// <remarks>
        /// Esta operacao deve proteger a geracao de `NumRevisao` contra colisoes concorrentes
        /// para o mesmo projeto.
        /// </remarks>
        /// <param name="revisao">Entidade de revisao a persistir.</param>
        /// <returns>Entidade persistida com `Revisao_id` e `NumRevisao` preenchidos.</returns>
        Task<Revisao> AddWithGeneratedNumeroAsync(Revisao revisao);
    }
}
