using System.ComponentModel.DataAnnotations;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.Dtos.FasesProducaoDto
{
    /// <summary>
    /// Contrato de entrada para criacao de uma fase de producao.
    /// </summary>
    public class CreateFasesProducaoDto
    {
        /// <summary>
        /// Nome funcional da fase.
        /// </summary>
        /// <remarks>
        /// O cliente tem de enviar este campo explicitamente.
        /// </remarks>
        [Required]
        public required Nome_fases Nome { get; set; }

        /// <summary>
        /// Descricao funcional opcional da fase.
        /// </summary>
        [MaxLength(255)]
        public string? Descricao { get; set; }
    }
}
