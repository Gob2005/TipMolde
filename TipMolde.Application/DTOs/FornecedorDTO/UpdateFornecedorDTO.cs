using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.Dtos.FornecedorDto
{
    /// <summary>
    /// DTO de entrada para atualizacao parcial de um fornecedor.
    /// </summary>
    /// <remarks>
    /// Todos os campos sao opcionais para permitir updates parciais sem obrigar o cliente a reenviar o registo completo.
    /// </remarks>
    public class UpdateFornecedorDto
    {
        [MinLength(3)]
        [MaxLength(100)]
        public string? Nome { get; set; }

        [MinLength(9)]
        [MaxLength(9)]
        public string? NIF { get; set; }

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