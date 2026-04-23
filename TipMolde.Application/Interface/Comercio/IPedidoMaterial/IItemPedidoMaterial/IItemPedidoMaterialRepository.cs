using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Interface.Comercio.IPedidoMaterial.IItemPedidoMaterial
{
    /// <summary>
    /// Define operacoes de persistencia especificas para linhas de pedido de material.
    /// </summary>
    public interface IItemPedidoMaterialRepository : IGenericRepository<ItemPedidoMaterial, int>
    {
        /// <summary>
        /// Lista linhas de um pedido de material.
        /// </summary>
        /// <param name="pedidoId">Identificador do pedido.</param>
        /// <param name="page">Numero da pagina a consultar.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Resultado paginado com linhas associadas ao pedido informado.</returns>
        Task<PagedResult<ItemPedidoMaterial>> GetByPedidoIdAsync(int pedidoId, int page = 1, int pageSize = 10);
    }
}
