using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.DTOs.FornecedorDTO
{
    /// <summary>
    /// DTO de entrada para criacao de um fornecedor no modulo comercial.
    /// </summary>
    /// <remarks>
    /// Agrega os dados base de identificacao e contacto necessarios para registo inicial do fornecedor.
    /// </remarks>
    public class CreateFornecedorDTO
    {
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [MinLength(9)]
        [MaxLength(9)]
        public string NIF { get; set; } = string.Empty;

        [MinLength(5)]
        [MaxLength(255)]
        public string? Morada { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Telefone { get; set; }
    }
}