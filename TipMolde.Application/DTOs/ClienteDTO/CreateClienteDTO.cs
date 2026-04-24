using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.Dtos.ClienteDto
{
    /// <summary>
    /// DTO de entrada para criacao de um cliente no modulo comercial.
    /// </summary>
    /// <remarks>
    /// Agrega os dados base de identificacao e contacto necessarios para registo inicial do cliente.
    /// </remarks>
    public class CreateClienteDto
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
