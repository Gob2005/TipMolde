using System.ComponentModel.DataAnnotations;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.Dtos.ProjetoDto
{
    /// <summary>
    /// Representa o pedido de criacao de um projeto de desenho.
    /// </summary>
    /// <remarks>
    /// O contrato inclui o caminho funcional da pasta no servidor, que passa a ser persistido no agregado.
    /// </remarks>
    public class CreateProjetoDto
    {
        [Required, MaxLength(100)]
        public required string NomeProjeto { get; set; }

        [Required, MaxLength(50)]
        public required string SoftwareUtilizado { get; set; }

        [Required]
        public TipoProjeto TipoProjeto { get; set; }

        [Required, MaxLength(255)]
        public required string CaminhoPastaServidor { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Molde_id { get; set; }
    }
}
