using System.ComponentModel.DataAnnotations;

namespace TipMolde.App.DTOs.ClienteDTO
{
    public class CreateClienteDTO
    {
        [Required, MinLength(5), MaxLength(100)]
        public required string Nome { get; set; }

        [MinLength(5), MaxLength(50)]
        public string? Pais { get; set; }

        [EmailAddress, MaxLength(150)]
        public string? Email { get; set; }

        [Phone, MaxLength(20)]
        public string? Telefone { get; set; }

        [Required, MinLength(9), MaxLength(20)]
        public required string NIF { get; set; }

        [Required, MaxLength(5)]
        public required string Sigla { get; set; }
    }
}
