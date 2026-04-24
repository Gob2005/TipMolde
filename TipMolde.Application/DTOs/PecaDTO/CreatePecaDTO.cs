using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.DTOs.PecaDTO
{
    /// <summary>
    /// Representa os dados de criacao de uma peca associada a um molde.
    /// </summary>
    /// <remarks>
    /// Este DTO pertence ao contrato de entrada da API e nao deve expor a entidade
    /// de dominio diretamente.
    /// </remarks>
    public class CreatePecaDTO
    {
        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public string Designacao { get; set; } = string.Empty;

        [Range(1, 9999)]
        public int Prioridade { get; set; }

        [MaxLength(100)]
        public string? MaterialDesignacao { get; set; }

        public bool MaterialRecebido { get; set; }

        [Range(1, int.MaxValue)]
        public int Molde_id { get; set; }
    }
}
