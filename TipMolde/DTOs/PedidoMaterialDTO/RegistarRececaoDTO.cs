using System.ComponentModel.DataAnnotations;

namespace TipMolde.API.DTOs.PedidoMaterialDTO
{
    public class RegistarRececaoDTO
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int UserId { get; set; }
    }
}
