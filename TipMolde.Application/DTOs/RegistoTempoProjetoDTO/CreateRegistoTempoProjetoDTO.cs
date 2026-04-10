using System.ComponentModel.DataAnnotations;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.DTOs.RegistoTempoProjetoDTO
{
    public class CreateRegistoTempoProjetoDTO
    {
        [Required]
        public EstadoTempoProjeto Estado_tempo { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Projeto_id { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Autor_id { get; set; }
    }
}
