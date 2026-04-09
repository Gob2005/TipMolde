namespace TipMolde.Core.Models.Producao
{
    /// <summary>
    /// Representa um componente individual de um molde.
    /// Cada peça passa por fases de produção rastreadas em RegistosProducao.
    /// </summary>
    /// <remarks>
    /// A designação é única dentro do contexto de um molde específico
    /// (validado por índice composto no ApplicationDbContext).
    /// </remarks>
    public class Peca
    {
        public int Peca_id { get; set; }

        /// <summary>
        /// Nome identificador da peça (ex: "Extrator", "Cavidade Superior").
        /// Deve ser único por molde para evitar confusão operacional.
        /// </summary>
        public required string Designacao { get; set; }

        /// <summary>
        /// Prioridade de produção dentro do molde (1 = mais prioritário).
        /// Usada para ordenação na fila de trabalho (RF-PR-07).
        /// </summary>
        public int Prioridade { get; set; }

        /// <summary>
        /// Designação do material necessário para produzir a peça.
        /// Referenciado em pedidos de material (ItemPedidoMaterial).
        /// </summary>
        public string? MaterialDesignacao { get; set; }

        /// <summary>
        /// Flag de bloqueio de produção.
        /// Se false, impede transição para estados produtivos (RF-CO-04).
        /// Atualizado automaticamente ao receber material (PedidoMaterialService).
        /// </summary>
        public bool MaterialRecebido { get; set; }

        public int Molde_id { get; set; }
        public Molde? Molde { get; set; }
    }
}