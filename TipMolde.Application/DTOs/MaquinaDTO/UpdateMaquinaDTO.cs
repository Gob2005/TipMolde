using System.ComponentModel.DataAnnotations;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.Dtos.MaquinaDto
{
    public class UpdateMaquinaDto
    {
        [MinLength(2)]
        [MaxLength(100)]
        public string? NomeModelo { get; set; }

        [MaxLength(45)]
        public string? IpAddress { get; set; }

        public EstadoMaquina Estado { get; set; }
    }
}