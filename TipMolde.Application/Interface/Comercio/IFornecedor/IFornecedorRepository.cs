using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Interface.Comercio.IFornecedor
{
    /// <summary>
    /// Define operacoes de persistencia de fornecedores no contexto comercial.
    /// </summary>
    /// <remarks>
    /// Expande o repositorio generico com consultas especificas de pesquisa textual e validacao de unicidade por NIF.
    /// </remarks>
    public interface IFornecedorRepository : IGenericRepository<Fornecedor, int>
    {
        /// <summary>
        /// Obtem um fornecedor pelo NIF.
        /// </summary>
        /// <param name="nif">Numero de identificacao fiscal do fornecedor.</param>
        /// <returns>Fornecedor encontrado ou nulo quando nao existe correspondencia.</returns>
        Task<Fornecedor?> GetByNifAsync(string nif);

        /// <summary>
        /// Pesquisa fornecedores por nome.
        /// </summary>
        /// <param name="searchTerm">Termo parcial para pesquisa no nome do fornecedor.</param>
        /// <param name="page">Numero da pagina a consultar.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Colecao de fornecedores ordenada alfabeticamente pelo nome.</returns>
        Task<PagedResult<Fornecedor>> SearchByNameAsync(string searchTerm, int page, int pageSize);
    }
}