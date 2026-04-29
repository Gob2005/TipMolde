using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Entities.Producao;

namespace TipMolde.Application.Interface.Producao.IMolde
{
    /// <summary>
    /// Define operacoes de persistencia especificas do agregado Molde.
    /// </summary>
    /// <remarks>
    /// As queries desta feature devem suportar leituras com especificacoes tecnicas
    /// e a criacao transacional do agregado principal com a respetiva associacao a encomenda.
    /// </remarks>
    public interface IMoldeRepository : IGenericRepository<Molde, int>
    {

        /// <summary>
        /// Obtem um molde pelo numero funcional.
        /// </summary>
        /// <param name="numero">Numero de negocio usado para identificar o molde.</param>
        /// <returns>Molde encontrado; nulo caso o numero nao exista.</returns>
        Task<Molde?> GetByNumeroAsync(string numero);

        /// <summary>
        /// Lista moldes associados a uma encomenda com paginacao.
        /// </summary>
        /// <param name="encomendaId">Identificador da encomenda usada como filtro.</param>
        /// <param name="page">Pagina atual da pesquisa.</param>
        /// <param name="pageSize">Numero maximo de registos por pagina.</param>
        /// <returns>Resultado paginado com os moldes ligados a encomenda indicada.</returns>
        Task<PagedResult<Molde>> GetByEncomendaIdAsync(int encomendaId, int page, int pageSize);


        /// <summary>
        /// Persiste o molde, as especificacoes tecnicas e a associacao inicial a encomenda.
        /// </summary>
        /// <remarks>
        /// Este metodo suporta o contrato de criacao do agregado completo quando a API
        /// recebe, no mesmo pedido, os dados do molde e da relacao EncomendaMolde.
        /// </remarks>
        /// <param name="molde">Entidade principal do agregado.</param>
        /// <param name="specs">Especificacoes tecnicas a associar ao molde criado.</param>
        /// <param name="link">Associacao inicial entre a encomenda e o molde.</param>
        /// <returns>Task de conclusao da operacao de persistencia.</returns>
        Task AddMoldeWithSpecsAndLinkAsync(Molde molde, EspecificacoesTecnicas specs, EncomendaMolde link);
    }
}
