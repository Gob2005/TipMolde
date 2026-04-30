using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Enums;

namespace TipMolde.Domain.Entities.Fichas
{
    /// <summary>
    /// Representa o cabecalho funcional comum das fichas editaveis de producao.
    /// </summary>
    /// <remarks>
    /// Esta entidade base concentra o estado documental, a ligacao ao contexto
    /// Encomenda-Molde e os metadados de auditoria partilhados por FRE, FRM, FRA e FOP.
    /// </remarks>
    public abstract class FichaProducao
    {
        /// <summary>
        /// Identificador interno da ficha de producao.
        /// </summary>
        public int FichaProducao_id { get; set; }
        /// <summary>
        /// Tipo documental da ficha a gerar e preencher.
        /// </summary>
        public TipoFicha Tipo { get; set; }
        /// <summary>
        /// Data de criacao do cabecalho da ficha.
        /// </summary>
        public DateTime DataCriacao { get; set; }

        /// <summary>
        /// Estado atual do ciclo de vida da ficha.
        /// </summary>
        public EstadoFichaProducao Estado { get; set; } = EstadoFichaProducao.RASCUNHO;
        /// <summary>
        /// Instante em que a ficha foi submetida.
        /// </summary>
        public DateTime? SubmetidaEm { get; set; }
        /// <summary>
        /// Utilizador responsavel pela submissao final da ficha.
        /// </summary>
        public int? SubmetidaPor_user_id { get; set; }
        /// <summary>
        /// Indica se a ficha continua ativa para consulta e manutencao.
        /// </summary>
        public bool Ativa { get; set; } = true;
        /// <summary>
        /// Instante em que a ficha foi desativada ou cancelada.
        /// </summary>
        public DateTime? DesativadaEm { get; set; }
        /// <summary>
        /// Utilizador responsavel pela desativacao logica da ficha.
        /// </summary>
        public int? DesativadaPor_user_id { get; set; }

        /// <summary>
        /// Identificador da associacao Encomenda-Molde a que a ficha pertence.
        /// </summary>
        public int EncomendaMolde_id { get; set; }
        /// <summary>
        /// Navegacao para o contexto comercial da ficha.
        /// </summary>
        public EncomendaMolde? EncomendaMolde { get; set; }

        /// <summary>
        /// Versoes documentais geradas ou anexadas para esta ficha.
        /// </summary>
        public ICollection<FichaDocumento> Relatorios { get; set; } = new List<FichaDocumento>();
    }
}
