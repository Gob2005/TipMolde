using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.Dtos.PedidoMaterialDto
{
    /// <summary>
    /// DTO de criacao de pedido de material.
    /// </summary>
    public class CreatePedidoMaterialDto
    {
        /// <summary>
        /// Identificador do fornecedor ao qual o pedido sera associado.
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int Fornecedor_id { get; set; }

        /// <summary>
        /// Linhas do pedido a persistir.
        /// </summary>
        [Required]
        [MinLength(1)]
        public List<CreateItemPedidoMaterialDto> Itens { get; set; } = new();
    }
}
