using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.DTOs.RevisaoDTO
{
    public class UpdateRespostaRevisaoDTO
    {
        [Required]
        public bool Aprovado { get; set; }

        [MaxLength(4000)]
        public string? FeedbackTexto { get; set; }

        [MaxLength(255)]
        public string? FeedbackImagemPath { get; set; }
    }
}
