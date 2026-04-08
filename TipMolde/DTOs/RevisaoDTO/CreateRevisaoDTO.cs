using System.ComponentModel.DataAnnotations;

namespace TipMolde.API.DTOs.RevisaoDTO
{
    public class CreateRevisaoDTO
    {
        [Required, MaxLength(2000)]
        public required string DescricaoAlteracoes { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Projeto_id { get; set; }
    }
}
