using System.ComponentModel.DataAnnotations;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.Dtos.ProjetoDto
{
    /// <summary>
    /// Representa o pedido de atualizacao parcial de um projeto.
    /// </summary>
    /// <remarks>
    /// Campos nao enviados devem preservar o valor atual do agregado.
    /// </remarks>
    public class UpdateProjetoDto
    {
        [MaxLength(100)]
        public string? NomeProjeto { get; set; }

        [MaxLength(50)]
        public string? SoftwareUtilizado { get; set; }

        public TipoProjeto? TipoProjeto { get; set; }

        [MaxLength(255)]
        public string? CaminhoPastaServidor { get; set; }
    }
}
