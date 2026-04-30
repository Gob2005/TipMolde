using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.Dtos.FichaProducaoDto
{
    /// <summary>
    /// Representa os dados de criacao ou atualizacao de uma linha da ficha FRM.
    /// </summary>
    public class CreateFichaFrmLinhaDto
    {
        [Required]
        public DateTime Data { get; set; }

        [Required, MaxLength(2000)]
        public required string Defeito { get; set; }

        [MaxLength(4000)]
        public string? Pormenor { get; set; }

        public bool Verificado { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Responsavel_id { get; set; }
    }
}
