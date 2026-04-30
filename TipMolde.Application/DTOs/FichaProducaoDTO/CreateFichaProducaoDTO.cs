using System.ComponentModel.DataAnnotations;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.Dtos.FichaProducaoDto
{
    /// <summary>
    /// Representa os dados minimos necessarios para criar uma ficha de producao.
    /// </summary>
    public class CreateFichaProducaoDto
    {
        [Required]
        public TipoFicha Tipo { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int EncomendaMolde_id { get; set; }
    }
}
