using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.DTOs.FornecedorDTO
{
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
        public string? Morada { get; set; } = string.Empty;

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Telefone { get; set; }
    }
}

