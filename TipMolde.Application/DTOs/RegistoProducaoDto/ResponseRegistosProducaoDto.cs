using TipMolde.Domain.Enums;

namespace TipMolde.Application.Dtos.RegistoProducaoDto
{
    /// <summary>
    /// Representa a resposta publica de um registo de producao.
    /// </summary>
    /// <remarks>
    /// Expoe apenas dados necessarios ao contrato HTTP e evita acoplamento direto
    /// entre consumidores da API e a entidade de dominio.
    /// </remarks>
    public class ResponseRegistosProducaoDto
    {
        /// <summary>
        /// Identificador unico do registo de producao.
        /// </summary>
        public int Registo_Producao_id { get; set; }

        /// <summary>
        /// Estado de producao atingido nesta transicao.
        /// </summary>
        public EstadoProducao Estado_producao { get; set; }

        /// <summary>
        /// Data e hora UTC em que o registo foi criado.
        /// </summary>
        public DateTime Data_hora { get; set; }

        /// <summary>
        /// Identificador da fase de producao associada.
        /// </summary>
        public int Fase_id { get; set; }

        /// <summary>
        /// Identificador do operador responsavel pela transicao.
        /// </summary>
        public int Operador_id { get; set; }

        /// <summary>
        /// Identificador da peca associada ao registo.
        /// </summary>
        public int Peca_id { get; set; }

        /// <summary>
        /// Identificador da maquina usada na transicao, quando aplicavel.
        /// </summary>
        public int? Maquina_id { get; set; }
    }
}
