using System.ComponentModel.DataAnnotations;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.Dtos.RegistoProducaoDto
{
    /// <summary>
    /// Contrato de entrada para criar um registo de producao.
    /// </summary>
    /// <remarks>
    /// Campos nullable distinguem omissao no payload de valores funcionais validos,
    /// evitando defaults tecnicos em regras criticas de producao.
    /// </remarks>
    public class CreateRegistosProducaoDto
    {
        /// <summary>
        /// Identificador da peca associada ao registo.
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int Peca_id { get; set; }

        /// <summary>
        /// Identificador da fase de producao onde ocorre a transicao.
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int Fase_id { get; set; }

        /// <summary>
        /// Identificador opcional da maquina associada ao registo.
        /// </summary>
        /// <remarks>
        /// Obrigatorio funcionalmente em PREPARACAO e EM_CURSO; em PAUSADO e CONCLUIDO
        /// pode ser inferido pelo ultimo registo da peca na fase.
        /// </remarks>
        [Range(1, int.MaxValue)]
        public int? Maquina_id { get; set; }

        /// <summary>
        /// Identificador do operador responsavel pela transicao.
        /// </summary>
        [Required]
        [Range(1, int.MaxValue)]
        public int Operador_id { get; set; }

        /// <summary>
        /// Estado de producao solicitado para a nova transicao.
        /// </summary>
        /// <remarks>
        /// Nullable para obrigar envio explicito do estado e evitar o default do enum.
        /// </remarks>
        [Required]
        public EstadoProducao? Estado_producao { get; set; }
    }
}
