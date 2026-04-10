using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.DTOs.FichaProducaoDTO
{
    public class CreateRegistoOcorrenciaDTO
    {
        [Required, MaxLength(4000)]
        public required string Descricao { get; set; }

        [MaxLength(4000)]
        public string? Correcao { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Responsavel_id { get; set; }
    }
}
