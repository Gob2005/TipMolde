using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.DTOs.ClienteDTO
{
    public class UpdateClienteDTO
    {
        [MinLength(3)]
        [MaxLength(100)]
        public string? Nome { get; set; }

        [MinLength(9)]
        [MaxLength(9)]
        public string? NIF { get; set; }

        [MinLength(2)]
        [MaxLength(10)]
        public string? Sigla { get; set; }

        [MaxLength(50)]
        public string? Pais { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Telefone { get; set; }
    }
}
