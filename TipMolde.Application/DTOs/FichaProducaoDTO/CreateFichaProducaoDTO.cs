using System.ComponentModel.DataAnnotations;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.DTOs.FichaProducaoDTO
{
    public class CreateFichaProducaoDTO
    {
        [Required]
        public TipoFicha Tipo { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int EncomendaMolde_id { get; set; }
    }
}
