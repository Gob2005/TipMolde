using TipMolde.Domain.Enums;

namespace TipMolde.Domain.Entities.Producao
{
    /// <summary>
    /// Regista uma transicao de estado de uma peca numa fase de producao.
    /// </summary>
    /// <remarks>
    /// Cada transicao gera um novo registo para manter rastreabilidade operacional.
    /// A data e definida pelo servico, e a maquina pode ser usada para sincronizar
    /// o estado operacional do equipamento com o historico de producao.
    /// </remarks>
    public class RegistosProducao
    {
        /// <summary>
        /// Identificador unico do registo de producao.
        /// </summary>
        public int Registo_Producao_id { get; set; }

        /// <summary>
        /// Estado de producao atingido nesta transicao.
        /// </summary>
        /// <remarks>
        /// Validado pela maquina de estados em RegistosProducaoService.
        /// </remarks>
        public EstadoProducao Estado_producao { get; set; } = EstadoProducao.PENDENTE;

        /// <summary>
        /// Data e hora UTC da transicao.
        /// </summary>
        /// <remarks>
        /// Definida automaticamente no servico para garantir timestamp confiavel.
        /// </remarks>
        public DateTime Data_hora { get; set; }

        /// <summary>
        /// Identificador da fase de producao associada.
        /// </summary>
        public int Fase_id { get; set; }

        /// <summary>
        /// Navegacao para a fase de producao associada.
        /// </summary>
        public FasesProducao? Fase { get; set; }

        /// <summary>
        /// Identificador do operador responsavel pela transicao.
        /// </summary>
        public int Operador_id { get; set; }

        /// <summary>
        /// Navegacao para o operador responsavel pela transicao.
        /// </summary>
        public User? Operador { get; set; }

        /// <summary>
        /// Identificador da peca associada ao registo.
        /// </summary>
        public int Peca_id { get; set; }

        /// <summary>
        /// Navegacao para a peca associada ao registo.
        /// </summary>
        public Peca? Peca { get; set; }

        /// <summary>
        /// Identificador da maquina utilizada na transicao, quando aplicavel.
        /// </summary>
        /// <remarks>
        /// Obrigatorio funcionalmente em PREPARACAO e EM_CURSO; em PAUSADO e CONCLUIDO
        /// pode ser inferido pelo ultimo registo de producao.
        /// </remarks>
        public int? Maquina_id { get; set; }

        /// <summary>
        /// Navegacao para a maquina utilizada na transicao.
        /// </summary>
        public Maquina? Maquina { get; set; }
    }
}
