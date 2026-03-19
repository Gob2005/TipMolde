using System.ComponentModel.DataAnnotations;
using TipMolde.Core.Enums;

namespace TipMolde.API.DTOs.EncomendaDTO
{
    public class UpdateEstadoEncomendaDTO
    {
        [Required]
        public required EstadoEncomenda Estado { get; set; }
    }
}
