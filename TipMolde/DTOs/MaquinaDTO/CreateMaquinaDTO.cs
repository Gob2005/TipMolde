using System.ComponentModel.DataAnnotations;
using TipMolde.Core.Enums;

namespace TipMolde.API.DTOs.MaquinaDTO
{
    public class CreateMaquinaDTO
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Maquina_id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string NomeModelo { get; set; } = string.Empty;

        [MaxLength(45)]
        public string? IpAddress { get; set; }

        [Required]
        public EstadoMaquina Estado { get; set; }
    }
}
