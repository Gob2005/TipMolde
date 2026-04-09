using TipMolde.Core.Models.Fichas;
using TipMolde.Core.Models.Producao;

namespace TipMolde.Core.Models.Comercio
{
    /// <summary>
    /// Tabela associativa entre Encomenda e Molde (relação N:M).
    /// Armazena atributos específicos da relação: quantidade, prioridade, prazo.
    /// </summary>
    /// <remarks>
    /// Permite que um molde apareça em múltiplas encomendas ao longo do tempo
    /// (cenário: reparação do mesmo molde em encomendas diferentes).
    /// </remarks>
    public class EncomendaMolde
    {
        public int EncomendaMolde_id { get; set; }

        /// <summary>
        /// Quantidade de unidades a produzir deste molde nesta encomenda.
        /// Usado para planeamento de capacidade.
        /// </summary>
        public int Quantidade { get; set; }

        /// <summary>
        /// Prioridade relativa dentro da encomenda (1 = mais prioritário).
        /// Determina ordem de produção quando há múltiplos moldes.
        /// </summary>
        public int Prioridade { get; set; }

        /// <summary>
        /// Prazo acordado com o cliente para este molde específico.
        /// Usado para cálculo de atrasos e indicadores de desempenho (KPI).
        /// </summary>
        public DateTime DataEntregaPrevista { get; set; }

        public int Encomenda_id { get; set; }
        public Encomenda? Encomenda { get; set; }

        public int Molde_id { get; set; }
        public Molde? Molde { get; set; }

        /// <summary>
        /// Fichas de produção associadas a este molde nesta encomenda.
        /// Requisito ISO 9001: documentação obrigatória por encomenda/molde.
        /// </summary>
        public ICollection<FichaProducao> Fichas { get; set; } = new List<FichaProducao>();
    }
}