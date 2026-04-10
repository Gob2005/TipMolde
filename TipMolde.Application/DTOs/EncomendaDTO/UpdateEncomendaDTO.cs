using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.DTOs.EncomendaDTO
{
    public class UpdateEncomendaDTO
    {
        [MinLength(1)]
        [MaxLength(50)]
        public string? NumeroEncomendaCliente { get; set; }

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
