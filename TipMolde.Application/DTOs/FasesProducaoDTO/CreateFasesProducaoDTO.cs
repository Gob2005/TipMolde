using System.ComponentModel.DataAnnotations;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.Dtos.Fases_producaoDto
{
    public class CreateFasesProducaoDto
    {
        [Required]
        public Nome_fases Nome { get; set; }

        [MaxLength(255)]
        public string? Descricao { get; set; }
    }
}
