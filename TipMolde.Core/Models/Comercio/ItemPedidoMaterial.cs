using TipMolde.Core.Models.Producao;

namespace TipMolde.Core.Models.Comercio
{
    public class ItemPedidoMaterial
    {
        public int ItemPedidoMaterial_id { get; set; }
        public int PedidoMaterial_id { get; set; }
        public PedidoMaterial? PedidoMaterial { get; set; }

        public int Peca_id { get; set; }
        public Peca? Peca { get; set; }

        public int Quantidade { get; set; }
    }
}
