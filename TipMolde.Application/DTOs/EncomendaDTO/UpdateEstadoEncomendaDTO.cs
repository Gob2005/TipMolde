using System.ComponentModel.DataAnnotations;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.DTOs.EncomendaDTO
{
    public class UpdateEstadoEncomendaDTO
    {
        [Required]
        public required EstadoEncomenda Estado { get; set; }
    }
}
