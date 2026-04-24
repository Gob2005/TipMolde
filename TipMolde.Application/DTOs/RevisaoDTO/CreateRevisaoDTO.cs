using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.Dtos.RevisaoDto
{
    /// <summary>
    /// Representa o pedido de criacao de uma revisao de desenho.
    /// </summary>
    /// <remarks>
    /// O payload identifica o projeto alvo e descreve as alteracoes
    /// que devem ser analisadas pelo cliente.
    /// </remarks>
    public class CreateRevisaoDto
    {
        [Required, MaxLength(2000)]
        public required string DescricaoAlteracoes { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int Projeto_id { get; set; }
    }
}
