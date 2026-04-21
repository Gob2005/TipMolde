using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Interface.Comercio.ICliente
{
    /// <summary>
    /// Define operacoes de persistencia de clientes no contexto comercial.
    /// </summary>
    /// <remarks>
    /// Expande o repositorio generico com consultas especificas de pesquisa e unicidade de cliente.
    /// </remarks>
    public interface IClienteRepository : IGenericRepository<Cliente, int>
    {
        /// <summary>
        /// Pesquisa clientes por nome.
        /// </summary>
        /// <param name="searchTerm">Termo parcial para pesquisa no nome do cliente.</param>
        /// <returns>Colecao de clientes ordenada por nome.</returns>
        Task<IEnumerable<Cliente>> SearchByNameAsync(string searchTerm);

        /// <summary>
        /// Pesquisa clientes por sigla.
        /// </summary>
        /// <param name="searchTerm">Termo parcial para pesquisa na sigla do cliente.</param>
        /// <returns>Colecao de clientes ordenada por sigla.</returns>
        Task<IEnumerable<Cliente>> SearchBySiglaAsync(string searchTerm);

        /// <summary>
        /// Obtem um cliente com a respetiva colecao de encomendas.
        /// </summary>
        /// <param name="clienteId">Identificador unico do cliente.</param>
        /// <returns>Cliente encontrado com encomendas ou nulo quando nao existe.</returns>
        Task<Cliente?> GetClienteWithEncomendasAsync(int clienteId);

        /// <summary>
        /// Obtem um cliente pelo NIF.
        /// </summary>
        /// <param name="nif">Numero de identificacao fiscal do cliente.</param>
        /// <returns>Cliente encontrado ou nulo quando nao existe correspondencia.</returns>
        Task<Cliente?> GetByNifAsync(string nif);

        /// <summary>
        /// Obtem um cliente pela sigla.
        /// </summary>
        /// <param name="sigla">Sigla identificadora do cliente.</param>
        /// <returns>Cliente encontrado ou nulo quando nao existe correspondencia.</returns>
        Task<Cliente?> GetBySiglaAsync(string sigla);
    }
}
