using TipMolde.Application.Dtos.ClienteDto;

namespace TipMolde.Application.Interface.Comercio.ICliente
{
    /// <summary>
    /// Define os casos de uso de negocio para gestao de clientes.
    /// </summary>
    /// <remarks>
    /// Centraliza operacoes de consulta, pesquisa, criacao, atualizacao e remocao de clientes.
    /// </remarks>
    public interface IClienteService
    {
        /// <summary>
        /// Lista clientes com paginacao.
        /// </summary>
        /// <param name="page">Numero da pagina solicitada.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Resultado paginado com clientes e metadados de navegacao.</returns>
        Task<PagedResult<ResponseClienteDto>> GetAllAsync(int page = 1, int pageSize = 10);

        /// <summary>
        /// Obtem um cliente pelo identificador.
        /// </summary>
        /// <param name="id">Identificador unico do cliente.</param>
        /// <returns>Cliente encontrado ou nulo quando nao existe registo.</returns>
        Task<ResponseClienteDto?> GetByIdAsync(int id);

        /// <summary>
        /// Obtem um cliente incluindo as encomendas associadas.
        /// </summary>
        /// <param name="clienteId">Identificador unico do cliente.</param>
        /// <returns>Cliente com encomendas ou nulo quando nao existe registo.</returns>
        Task<ResponseClienteWithEncomendasDto?> GetClienteWithEncomendasAsync(int clienteId);

        /// <summary>
        /// Pesquisa clientes por nome.
        /// </summary>
        /// <param name="searchTerm">Termo parcial de pesquisa por nome.</param>
        /// <param name="page">Numero da pagina solicitada.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Colecao de clientes que correspondem ao termo informado.</returns>
        Task<PagedResult<ResponseClienteDto>> SearchByNameAsync(string searchTerm, int page = 1, int pageSize = 10);

        /// <summary>
        /// Pesquisa clientes por sigla.
        /// </summary>
        /// <param name="searchTerm">Termo parcial de pesquisa por sigla.</param>
        /// <param name="page">Numero da pagina solicitada.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Colecao de clientes que correspondem ao termo informado.</returns>
        Task<PagedResult<ResponseClienteDto>> SearchBySiglaAsync(string searchTerm, int page = 1, int pageSize = 10);

        /// <summary>
        /// Cria um novo cliente.
        /// </summary>
        /// <param name="dto">DTO com dados do cliente a persistir.</param>
        /// <returns>DTO de resposta do cliente apos validacao e persistencia.</returns>
        Task<ResponseClienteDto> CreateAsync(CreateClienteDto dto);

        /// <summary>
        /// Atualiza os dados de um cliente existente.
        /// </summary>
        /// <param name="id">Identificador unico do cliente a atualizar.</param>
        /// <param name="dto">DTO com os dados a atualizar no cliente.</param>
        /// <returns>Task assincrona concluida apos atualizacao do cliente.</returns>
        Task UpdateAsync(int id, UpdateClienteDto dto);

        /// <summary>
        /// Remove um cliente pelo identificador.
        /// </summary>
        /// <param name="id">Identificador unico do cliente a remover.</param>
        /// <returns>Task assincrona concluida apos remocao do cliente.</returns>
        Task DeleteAsync(int id);
    }
}
