using System.ComponentModel.DataAnnotations;

namespace TipMolde.Core.Models
{
    public class EspecificacoesTecnicas
    {
        [Key]
        public int Molde_id { get; set; }
        public Molde? Molde { get; set; }

        public decimal? Largura { get; set; }
        public decimal? Comprimento { get; set; }
        public decimal? Altura { get; set; }
        public decimal? PesoEstimado { get; set; }
        public string? TipoInjecao { get; set; }
        public string? SistemaInjecao { get; set; }
        public decimal? Contracao { get; set; }
        public string? AcabamentoPeca { get; set; }
        public string? Cor { get; set; }
        public string? MaterialMacho { get; set; }
        public string? MaterialCavidade { get; set; }
        public string? MaterialMovimentos { get; set; }
        public string? MaterialInjecao { get; set; }
    }

}
