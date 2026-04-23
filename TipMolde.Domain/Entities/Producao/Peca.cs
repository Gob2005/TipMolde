namespace TipMolde.Domain.Entities.Producao
{
    /// <summary>
    /// Representa um componente individual de um molde.
    /// </summary>
    /// <remarks>
    /// Cada peca passa por fases de producao rastreadas em RegistosProducao e
    /// pode ficar bloqueada ate o material associado ser recebido.
    /// </remarks>
    public class Peca
    {
        public int Peca_id { get; set; }

        /// <summary>
        /// Nome identificador da peca dentro do molde.
        /// </summary>
        public required string Designacao { get; set; }

        /// <summary>
        /// Prioridade de producao dentro do molde.
        /// </summary>
        public int Prioridade { get; set; }

        /// <summary>
        /// Designacao do material necessario para produzir a peca.
        /// </summary>
        public string? MaterialDesignacao { get; set; }

        /// <summary>
        /// Flag de bloqueio de producao.
        /// Fica a true quando a rececao do material e registada para o pedido associado.
        /// </summary>
        public bool MaterialRecebido { get; set; }

        public int Molde_id { get; set; }
        public Molde? Molde { get; set; }
    }
}
