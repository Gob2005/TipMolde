using TipMolde.Domain.Enums;
using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Domain.Entities.Producao
{
    /// <summary>
    /// Representa um molde para injeção de plástico em produção.
    /// Entidade central do sistema, ligada a encomendas, peças e projetos.
    /// </summary>
    /// <remarks>
    /// O Numero é único e usado como identificador principal pelos operadores.
    /// NumeroMoldeCliente permite rastreabilidade com sistemas externos do cliente.
    /// </remarks>
    public class Molde
    {
        public int Molde_id { get; set; }

        /// <summary>
        /// Número interno único do molde (ex: "M-001").
        /// Indexado para pesquisas rápidas.
        /// </summary>
        public required string Numero { get; set; }

        /// <summary>
        /// Número de referência do molde no sistema do cliente.
        /// Pode ser null se não fornecido.
        /// </summary>
        public string? NumeroMoldeCliente { get; set; }

        public string? Nome { get; set; }
        public string? Descricao { get; set; }

        /// <summary>
        /// Número de cavidades do molde.
        /// Determina a capacidade de produção em série.
        /// Validado >= 1 nos DTOs.
        /// </summary>
        public int Numero_cavidades { get; set; }

        /// <summary>
        /// Tipo de trabalho a realizar: novo molde, reparação ou alteração.
        /// Determina o fluxo de aprovação e fases aplicáveis.
        /// </summary>
        public TipoPedido TipoPedido { get; set; }

        /// <summary>
        /// Caminho relativo para a imagem de capa do molde.
        /// Usado em relatórios e fichas de produção (FLT, FRE).
        /// </summary>
        public string? ImagemCapaPath { get; set; }

        /// <summary>
        /// Especificações técnicas detalhadas (relação 1:1).
        /// Separadas para evitar poluição desta tabela.
        /// </summary>
        public EspecificacoesTecnicas? Especificacoes { get; set; }

        /// <summary>
        /// Peças que compõem este molde.
        /// Cada peça passa por fases de produção independentemente.
        /// </summary>
        public ICollection<Peca> Pecas { get; set; } = new List<Peca>();

        /// <summary>
        /// Ligação N:M com encomendas através de EncomendaMolde.
        /// Permite molde em múltiplas encomendas (reparações, alterações).
        /// </summary>
        public ICollection<EncomendaMolde> EncomendasMoldes { get; set; } = new List<EncomendaMolde>();
    }
}