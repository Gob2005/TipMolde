using TipMolde.Core.Enums;

namespace TipMolde.Core.Models.Comercio
{
    /// <summary>
    /// Representa uma compra de matéria-prima a um fornecedor.
    /// Controla o ciclo: pedido → receção → desbloqueio de produção.
    /// </summary>
    /// <remarks>
    /// Estado transita de "Pendente" para "Recebido" ao registar receção (RF-CO-03).
    /// Ao receber, atualiza automaticamente Peca.MaterialRecebido para desbloquear produção.
    /// </remarks>
    public class PedidoMaterial
    {
        public int PedidoMaterial_id { get; set; }

        /// <summary>
        /// Data em que o pedido foi criado no sistema.
        /// Definida automaticamente em CreateAsync.
        /// </summary>
        public DateTime DataPedido { get; set; }

        /// <summary>
        /// Data em que o material foi recebido e conferido.
        /// Null enquanto pedido está pendente.
        /// </summary>
        public DateTime? DataRececao { get; set; }

        /// <summary>
        /// Estado do pedido: "Pendente" ou "Recebido".
        /// </summary>
        public EstadoPedido Estado { get; set; } = EstadoPedido.PENDENTE;

        public int Fornecedor_id { get; set; }
        public Fornecedor? Fornecedor { get; set; }

        /// <summary>
        /// Utilizador que conferiu a receção do material (ISO 9001: rastreabilidade).
        /// Null enquanto não recebido.
        /// </summary>
        public int? UserConferente_id { get; set; }
        public User? Conferente { get; set; }

        /// <summary>
        /// Itens (peças) incluídos neste pedido.
        /// Relação N:M com Peca através de ItemPedidoMaterial.
        /// </summary>
        public ICollection<ItemPedidoMaterial> Itens { get; set; } = new List<ItemPedidoMaterial>();
    }
}