using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.DTOs.ClienteDTO
{
    public class CreateClienteDTO
    {
        [Required]
        [MinLength(3)]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [MinLength(9)]
        [MaxLength(9)]
        public string NIF { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        [MaxLength(10)]
        public string Sigla { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Pais { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Telefone { get; set; }
    }
}
