using System.ComponentModel.DataAnnotations;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.Dtos.EncomendaDto
{
    public class UpdateEstadoEncomendaDto
    {
        [Required]
        public required EstadoEncomenda Estado { get; set; }
    }
}
