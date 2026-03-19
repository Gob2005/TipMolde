using System.ComponentModel.DataAnnotations;

namespace TipMolde.API.DTOs.EncomendaDTO
{
    public class CreateEncomendaDTO
    {
        [Required]
        public int Cliente_id { get; set; }


        [Required, MinLength(3), MaxLength(50)]
        public required string NumeroEncomendaCliente { get; set; }
    }
}
