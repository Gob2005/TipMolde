using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Entities.Producao;

namespace TipMolde.Application.Interface.Comercio.IPedidoMaterial
{
    /// <summary>
    /// Define operacoes de persistencia especificas do agregado PedidoMaterial.
    /// </summary>
    public interface IPedidoMaterialRepository : IGenericRepository<PedidoMaterial, int>
    {
        /// <summary>
        /// Lista pedidos de material com paginacao, incluindo as respetivas linhas.
        /// </summary>
        /// <param name="page">Numero da pagina a consultar.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Resultado paginado com pedidos enriquecidos com itens.</returns>
        Task<PagedResult<PedidoMaterial>> GetPagedWithItensAsync(int page, int pageSize);

        /// <summary>
        /// Obtem um pedido de material pelo identificador, incluindo as respetivas linhas.
        /// </summary>
        /// <param name="id">Identificador unico do pedido.</param>
        /// <returns>Pedido encontrado ou nulo quando nao existe registo.</returns>
        Task<PedidoMaterial?> GetByIdWithItensAsync(int id);

        /// <summary>
        /// Lista pedidos de material de um fornecedor, incluindo as respetivas linhas.
        /// </summary>
        /// <param name="fornecedorId">Identificador do fornecedor.</param>
        /// <param name="page">Numero da pagina a consultar.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Resultado paginado com pedidos associados ao fornecedor.</returns>
        Task<PagedResult<PedidoMaterial>> GetByFornecedorIdWithItensAsync(int fornecedorId, int page, int pageSize);

        /// <summary>
        /// Persiste de forma consistente a rececao do pedido e o desbloqueio das pecas associadas.
        /// </summary>
        /// <param name="pedido">Pedido ja atualizado com estado, data e utilizador conferente.</param>
        /// <param name="pecas">Colecao de pecas a marcar com material recebido.</param>
        /// <returns>Task assincrona concluida apos commit atomico.</returns>
        Task RegistarRececaoAsync(PedidoMaterial pedido, IEnumerable<Peca> pecas);
    }
}
