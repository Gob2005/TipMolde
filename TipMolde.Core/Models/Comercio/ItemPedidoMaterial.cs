using TipMolde.Core.Models.Producao;

namespace TipMolde.Core.Models.Comercio
{
    /// <summary>
    /// Linha de um pedido de material (relação N:M entre PedidoMaterial e Peca).
    /// Armazena a quantidade de material solicitado para cada peça.
    /// </summary>
    /// <remarks>
    /// Chave composta (PedidoMaterial_id, Peca_id) definida no ApplicationDbContext.
    /// Permite rastrear quais materiais foram pedidos para quais peças específicas.
    /// </remarks>
    public class ItemPedidoMaterial
    {
        public int ItemPedidoMaterial_id { get; set; }

        public int PedidoMaterial_id { get; set; }
        public PedidoMaterial? PedidoMaterial { get; set; }

        public int Peca_id { get; set; }
        public Peca? Peca { get; set; }

        /// <summary>
        /// Quantidade de material solicitado.
        /// Unidade depende do tipo de material (kg, unidades, metros).
        /// Deveria ser acompanhado por unidade de medida.
        /// </summary>
        public int Quantidade { get; set; }
    }
}