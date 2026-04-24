using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.Dtos.EncomendaMoldeDto
{
    public class UpdateEncomendaMoldeDto
    {
        [Range(1, 999999)]
        public int? Quantidade { get; set; }

        [Range(1, 9999)]
        public int? Prioridade { get; set; }

        public DateTime? DataEntregaPrevista { get; set; }
    }
}
