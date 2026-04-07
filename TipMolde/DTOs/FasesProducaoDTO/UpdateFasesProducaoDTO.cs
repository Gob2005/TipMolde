using System.ComponentModel.DataAnnotations;
using TipMolde.Core.Enums;

namespace TipMolde.API.DTOs.Fases_producaoDTO
{
    public class UpdateFasesProducaoDTO
    {
        public Nome_fases? Nome { get; set; }

        [MaxLength(255)]
        public string? Descricao { get; set; }
    }
}
