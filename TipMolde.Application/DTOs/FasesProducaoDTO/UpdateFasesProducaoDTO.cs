using System.ComponentModel.DataAnnotations;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.Dtos.FasesProducaoDto
{
    /// <summary>
    /// Contrato de entrada para atualizacao parcial de uma fase de producao.
    /// </summary>
    public class UpdateFasesProducaoDto
    {
        /// <summary>
        /// Novo nome funcional da fase.
        /// </summary>
        public Nome_fases? Nome { get; set; }

        /// <summary>
        /// Nova descricao funcional da fase.
        /// </summary>
        [MaxLength(255)]
        public string? Descricao { get; set; }
    }
}
