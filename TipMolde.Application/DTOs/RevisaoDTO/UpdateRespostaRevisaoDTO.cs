using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.DTOs.RevisaoDTO
{
    /// <summary>
    /// Representa o pedido de resposta do cliente a uma revisao enviada.
    /// </summary>
    /// <remarks>
    /// Quando a revisao e rejeitada, o pedido deve incluir justificacao textual
    /// ou referencia para evidencia anexa, para preservar contexto funcional.
    /// </remarks>
    public class UpdateRespostaRevisaoDTO : IValidatableObject
    {
        [Required]
        public bool? Aprovado { get; set; }

        [MaxLength(4000)]
        public string? FeedbackTexto { get; set; }

        [MaxLength(255)]
        public string? FeedbackImagemPath { get; set; }

        /// <summary>
        /// Valida regras de negocio do payload de resposta do cliente.
        /// </summary>
        /// <param name="validationContext">Contexto de validacao do modelo.</param>
        /// <returns>Colecao de erros de validacao encontrados.</returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Aprovado == false
                && string.IsNullOrWhiteSpace(FeedbackTexto)
                && string.IsNullOrWhiteSpace(FeedbackImagemPath))
            {
                yield return new ValidationResult(
                    "Quando a revisao e rejeitada, deve ser enviado FeedbackTexto ou FeedbackImagemPath.",
                    new[] { nameof(FeedbackTexto), nameof(FeedbackImagemPath) });
            }
        }
    }
}
