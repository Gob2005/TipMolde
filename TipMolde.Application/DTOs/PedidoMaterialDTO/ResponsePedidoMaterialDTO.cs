using TipMolde.Domain.Enums;

namespace TipMolde.Application.Dtos.PedidoMaterialDto
{
    /// <summary>
    /// DTO de resposta do agregado pedido de material.
    /// </summary>
    public class ResponsePedidoMaterialDto
    {
        /// <summary>
        /// Identificador unico do pedido.
        /// </summary>
        public int PedidoMaterialId { get; set; }

        /// <summary>
        /// Data de registo do pedido no sistema.
        /// </summary>
        public DateTime DataPedido { get; set; }

        /// <summary>
        /// Data de rececao do material.
        /// </summary>
        public DateTime? DataRececao { get; set; }

        /// <summary>
        /// Estado funcional do pedido.
        /// </summary>
        public EstadoPedido Estado { get; set; }

        /// <summary>
        /// Identificador do fornecedor associado.
        /// </summary>
        public int FornecedorId { get; set; }

        /// <summary>
        /// Identificador do utilizador que conferiu a rececao.
        /// </summary>
        public int? UserConferenteId { get; set; }

        /// <summary>
        /// Linhas do pedido.
        /// </summary>
        public List<ResponseItemPedidoMaterialDto> Itens { get; set; } = new();
    }
}
