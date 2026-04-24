using System.ComponentModel.DataAnnotations;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.Dtos.RegistoProducaoDto
{
    public class CreateRegistosProducaoDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Peca_id { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Fase_id { get; set; }

        [Range(1, int.MaxValue)]
        public int Maquina_id { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Operador_id { get; set; }

        [Required]
        public EstadoProducao Estado_producao { get; set; }
    }
}
