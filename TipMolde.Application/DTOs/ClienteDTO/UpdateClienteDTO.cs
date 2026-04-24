using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.Dtos.ClienteDto
{
    /// <summary>
    /// DTO de entrada para atualizacao parcial de um cliente existente.
    /// </summary>
    /// <remarks>
    /// Todos os campos sao opcionais para permitir alteracao apenas da informacao que foi enviada no pedido.
    /// </remarks>
    public class UpdateClienteDto
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
