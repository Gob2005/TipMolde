using System.ComponentModel.DataAnnotations;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.DTOs.Fases_producaoDTO
{
    public class CreateFasesProducaoDTO
    {
        [Required]
        public Nome_fases Nome { get; set; }

        [MaxLength(255)]
        public string? Descricao { get; set; }
    }
}
