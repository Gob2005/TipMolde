using System.ComponentModel.DataAnnotations;
using TipMolde.Core.Enums;

namespace TipMolde.API.DTOs.MoldeDTO
{
    public class CreateMoldeDTO
    {
        [Required]
        [MinLength(1)]
        [MaxLength(20)]
        public string Numero { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? NumeroMoldeCliente { get; set; }

        [MaxLength(100)]
        public string? Nome { get; set; }

        [MaxLength(1000)]
        public string? Descricao { get; set; }

        [Required]
        [Range(1, 9999)]
        public int Numero_cavidades { get; set; }

        [Required]
        public TipoPedido TipoPedido { get; set; }

        [Range(0, 999999.99)]
        public decimal? Largura { get; set; }

        [Range(0, 999999.99)]
        public decimal? Comprimento { get; set; }

        [Range(0, 999999.99)]
        public decimal? Altura { get; set; }

        [Range(0, 999999.99)]
        public decimal? PesoEstimado { get; set; }

        [MaxLength(50)]
        public string? TipoInjecao { get; set; }

        [MaxLength(50)]
        public string? SistemaInjecao { get; set; }

        [Range(0, 999.99)]
        public decimal? Contracao { get; set; }

        [MaxLength(50)]
        public string? AcabamentoPeca { get; set; }

        [MaxLength(30)]
        public string? Cor { get; set; }

        [MaxLength(50)]
        public string? MaterialMacho { get; set; }

        [MaxLength(50)]
        public string? MaterialCavidade { get; set; }

        [MaxLength(50)]
        public string? MaterialMovimentos { get; set; }

        [MaxLength(50)]
        public string? MaterialInjecao { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int EncomendaId { get; set; }

        [Required]
        [Range(1, 999999)]
        public int Quantidade { get; set; }

        [Required]
        [Range(1, 9999)]
        public int Prioridade { get; set; }

        [Required]
        public DateTime DataEntregaPrevista { get; set; }
    }
}
