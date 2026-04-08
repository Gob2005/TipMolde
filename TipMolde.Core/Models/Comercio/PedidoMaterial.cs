namespace TipMolde.Core.Models.Comercio
{
    public class PedidoMaterial
    {
        public int PedidoMaterial_id { get; set; }
        public DateTime DataPedido { get; set; }
        public DateTime? DataRececao { get; set; }
        public string Estado { get; set; } = "Pendente";

        public int Fornecedor_id { get; set; }
        public Fornecedor? Fornecedor { get; set; }

        public int? UserConferente_id { get; set; }
        public User? Conferente { get; set; }

        public ICollection<ItemPedidoMaterial> Itens { get; set; } = new List<ItemPedidoMaterial>();
    }
}
