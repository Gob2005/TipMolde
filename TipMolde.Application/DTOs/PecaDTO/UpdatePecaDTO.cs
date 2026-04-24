using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.DTOs.PecaDTO
{
    /// <summary>
    /// Representa os dados de atualizacao parcial de uma peca.
    /// </summary>
    /// <remarks>
    /// Campos omitidos devem preservar o valor atual da entidade.
    /// </remarks>
    public class UpdatePecaDTO
    {
        [MinLength(2)]
        [MaxLength(100)]
        public string? Designacao { get; set; }

        [Range(1, 9999)]
        public int? Prioridade { get; set; }

        [MaxLength(100)]
        public string? MaterialDesignacao { get; set; }

        public bool? MaterialRecebido { get; set; }
    }
}
