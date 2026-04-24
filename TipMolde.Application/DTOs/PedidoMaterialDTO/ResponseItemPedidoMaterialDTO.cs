namespace TipMolde.Application.Dtos.PedidoMaterialDto
{
    /// <summary>
    /// DTO de resposta para uma linha de pedido de material.
    /// </summary>
    public class ResponseItemPedidoMaterialDto
    {
        /// <summary>
        /// Identificador tecnico da linha.
        /// </summary>
        public int ItemId { get; set; }

        /// <summary>
        /// Identificador da peca associada a linha.
        /// </summary>
        public int PecaId { get; set; }

        /// <summary>
        /// Quantidade pedida para a peca.
        /// </summary>
        public int Quantidade { get; set; }
    }
}
