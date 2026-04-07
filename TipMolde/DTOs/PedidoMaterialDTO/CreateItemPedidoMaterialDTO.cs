using System.ComponentModel.DataAnnotations;

namespace TipMolde.API.DTOs.PedidoMaterialDTO
{
    public class CreateItemPedidoMaterialDTO
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Peca_id { get; set; }

        [Required]
        [Range(1, 999999)]
        public int Quantidade { get; set; }
    }
}
