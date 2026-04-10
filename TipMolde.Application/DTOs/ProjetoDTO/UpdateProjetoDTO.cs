using System.ComponentModel.DataAnnotations;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.DTOs.ProjetoDTO
{
    public class UpdateProjetoDTO
    {
        [MaxLength(100)]
        public string? NomeProjeto { get; set; }

        [MaxLength(50)]
        public string? SoftwareUtilizado { get; set; }

        public TipoProjeto? TipoProjeto { get; set; }

        [MaxLength(255)]
        public string? CaminhoPastaServidor { get; set; }
    }
}
