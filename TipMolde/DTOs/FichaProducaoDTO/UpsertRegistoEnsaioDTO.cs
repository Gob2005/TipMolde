using System.ComponentModel.DataAnnotations;

namespace TipMolde.API.DTOs.FichaProducaoDTO
{
    public class UpsertRegistoEnsaioDTO
    {
        [Required, MaxLength(100)]
        public required string LocalEnsaio { get; set; }

        public bool AguasCavidade { get; set; }
        public bool AguasMacho { get; set; }
        public bool AguasMovimentos { get; set; }

        [MaxLength(4000)]
        public string? ResumoTexto { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Maquina_id { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Responsavel_id { get; set; }
    }
}
