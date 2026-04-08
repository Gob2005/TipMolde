using System.ComponentModel.DataAnnotations;
using TipMolde.Core.Enums;

namespace TipMolde.API.DTOs.ProjetoDTO
{
    public class CreateProjetoDTO
    {
        [Required, MaxLength(100)]
        public required string NomeProjeto { get; set; }

        [Required, MaxLength(50)]
        public required string SoftwareUtilizado { get; set; }

        [Required]
        public TipoProjeto TipoProjeto { get; set; }

        [Required, MaxLength(255)]
        public required string CaminhoPastaServidor { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Molde_id { get; set; }
    }
}
