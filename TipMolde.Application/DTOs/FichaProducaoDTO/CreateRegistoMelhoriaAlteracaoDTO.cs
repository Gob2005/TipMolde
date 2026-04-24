using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.Dtos.FichaProducaoDto
{
    public class CreateRegistoMelhoriaAlteracaoDto
    {
        [Required, MaxLength(2000)]
        public required string ItemDescricao { get; set; }

        [MaxLength(4000)]
        public string? Pormenor { get; set; }

        public bool Verificado { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Responsavel_id { get; set; }
    }
}
