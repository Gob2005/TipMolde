using TipMolde.Application.DTOs.PedidoMaterialDTO;
using TipMolde.Application.Interface;

namespace TipMolde.Application.Interface.Comercio.IPedidoMaterial
{
    /// <summary>
    /// Define os casos de uso de negocio do agregado PedidoMaterial.
    /// </summary>
    /// <remarks>
    /// O contrato da camada Application devolve DTOs estaveis para evitar acoplamento da API ao modelo interno.
    /// </remarks>
    public interface IPedidoMaterialService
    {
        /// <summary>
        /// Lista pedidos de material com paginacao.
        /// </summary>
        /// <param name="page">Numero da pagina solicitada.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Resultado paginado com pedidos de material e metadados de navegacao.</returns>
        Task<PagedResult<ResponsePedidoMaterialDTO>> GetAllAsync(int page = 1, int pageSize = 10);

        /// <summary>
        /// Obtem um pedido de material pelo identificador.
        /// </summary>
        /// <param name="id">Identificador unico do pedido.</param>
        /// <returns>DTO do pedido encontrado ou nulo quando nao existe registo.</returns>
        Task<ResponsePedidoMaterialDTO?> GetByIdAsync(int id);

        /// <summary>
        /// Lista pedidos de material de um fornecedor.
        /// </summary>
        /// <param name="fornecedorId">Identificador do fornecedor.</param>
        /// <param name="page">Numero da pagina solicitada.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Resultado paginado com pedidos associados ao fornecedor informado.</returns>
        Task<PagedResult<ResponsePedidoMaterialDTO>> GetByFornecedorIdAsync(int fornecedorId, int page = 1, int pageSize = 10);

        /// <summary>
        /// Cria um novo pedido de material.
        /// </summary>
        /// <param name="dto">DTO com os dados do pedido e das respetivas linhas.</param>
        /// <returns>DTO do pedido criado apos validacao e persistencia.</returns>
        Task<ResponsePedidoMaterialDTO> CreateAsync(CreatePedidoMaterialDTO dto);

        /// <summary>
        /// Regista a rececao de um pedido de material.
        /// </summary>
        /// <param name="pedidoId">Identificador do pedido a marcar como recebido.</param>
        /// <param name="userId">Identificador do utilizador autenticado que conferiu a rececao.</param>
        /// <returns>Task assincrona concluida apos atualizacao consistente do pedido e das pecas.</returns>
        Task RegistarRececaoAsync(int pedidoId, int userId);

        /// <summary>
        /// Remove um pedido de material.
        /// </summary>
        /// <param name="id">Identificador unico do pedido a remover.</param>
        /// <returns>Task assincrona concluida apos remocao do pedido.</returns>
        Task DeleteAsync(int id);
    }
}
