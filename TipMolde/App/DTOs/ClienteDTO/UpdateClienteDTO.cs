using System.ComponentModel.DataAnnotations;

namespace TipMolde.App.DTOs.ClienteDTO
{
    public class UpdateClienteDTO
    {
        [MinLength(5), MaxLength(100)]
        public string? Nome { get; set; }

        [MinLength(5), MaxLength(50)]
        public string? Pais { get; set; }

        [EmailAddress, MaxLength(150)]
        public string? Email { get; set; }

        [Phone, MaxLength(20)]
        public string? Telefone { get; set; }

        [Required, MinLength(9), MaxLength(20)]
        public string? NIF { get; set; }

        [Required, MaxLength(5)]
        public string? Sigla { get; set; }
    }
}
