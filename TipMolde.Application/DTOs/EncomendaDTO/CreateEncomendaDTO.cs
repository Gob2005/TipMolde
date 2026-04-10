using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.DTOs.EncomendaDTO
{
    public class CreateEncomendaDTO
    {
        [Required]
        public int Cliente_id { get; set; }


        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        public required string NumeroEncomendaCliente { get; set; }

        [MinLength(1)]
        [MaxLength(50)]
        public string? NumeroProjetoCliente { get; set; }

        [MinLength(1)]
        [MaxLength(100)]
        public string? NomeServicoCliente { get; set; }

        [MinLength(1)]
        [MaxLength(100)]
        public string? NomeResponsavelCliente { get; set; }
    }
}
