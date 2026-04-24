using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.Dtos.PedidoMaterialDto
{
    /// <summary>
    /// Representa uma linha de criacao de pedido de material.
    /// </summary>
    public class CreateItemPedidoMaterialDto
    {
        /// <summary>
        /// Identificador da peca que necessita de material.
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int Peca_id { get; set; }

        /// <summary>
        /// Quantidade solicitada para a peca indicada.
        /// </summary>
        [Required]
        [Range(1, 999999)]
        public int Quantidade { get; set; }
    }
}
