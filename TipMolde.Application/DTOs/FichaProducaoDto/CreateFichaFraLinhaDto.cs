using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.Dtos.FichaProducaoDto
{
    /// <summary>
    /// Representa os dados de criacao ou atualizacao de uma linha da ficha FRA.
    /// </summary>
    public class CreateFichaFraLinhaDto
    {
        [Required]
        public DateTime Data { get; set; }

        [Required, MaxLength(4000)]
        public required string Alteracoes { get; set; }

        public bool Verificado { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Responsavel_id { get; set; }
    }
}
