using System.ComponentModel.DataAnnotations;

namespace TipMolde.API.DTOs.EncomendaMoldeDTO
{
    public class CreateEncomendaMoldeDTO
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Encomenda_id { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Molde_id { get; set; }

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
