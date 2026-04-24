namespace TipMolde.Domain.Entities.Producao
{
    /// <summary>
    /// Representa um componente individual de um molde.
    /// </summary>
    /// <remarks>
    /// A entidade guarda os dados funcionais da peca usados no planeamento e
    /// no acompanhamento da producao dentro do respetivo molde.
    /// </remarks>
    public class Peca
    {
        public int Peca_id { get; set; }

        /// <summary>
        /// Designacao funcional unica da peca dentro do mesmo molde.
        /// </summary>
        public required string Designacao { get; set; }

        /// <summary>
        /// Prioridade relativa da peca no planeamento de producao.
        /// </summary>
        public int Prioridade { get; set; }

        /// <summary>
        /// Designacao do material previsto para fabricar a peca.
        /// </summary>
        public string? MaterialDesignacao { get; set; }

        /// <summary>
        /// Indica se o material necessario para a peca ja foi rececionado.
        /// </summary>
        public bool MaterialRecebido { get; set; }

        public int Molde_id { get; set; }
        public Molde? Molde { get; set; }
    }
}
