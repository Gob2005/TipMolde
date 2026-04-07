using System.ComponentModel.DataAnnotations;

namespace TipMolde.API.DTOs.PedidoMaterialDTO
{
    public class CreatePedidoMaterialDTO
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Fornecedor_id { get; set; }

        [Required]
        [MinLength(1)]
        public List<CreateItemPedidoMaterialDTO> Itens { get; set; } = new();
    }
}
