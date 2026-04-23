using TipMolde.Domain.Enums;

namespace TipMolde.Domain.Entities.Comercio
{
    /// <summary>
    /// Representa uma compra de materia-prima a um fornecedor.
    /// </summary>
    /// <remarks>
    /// O ciclo funcional deste agregado e:
    /// 1. Criacao do pedido com linhas associadas a pecas.
    /// 2. Rececao unica do material.
    /// 3. Desbloqueio das pecas para producao atraves de MaterialRecebido = true.
    /// </remarks>
    public class PedidoMaterial
    {
        /// <summary>
        /// Identificador unico do pedido.
        /// </summary>
        public int PedidoMaterial_id { get; set; }

        /// <summary>
        /// Data em que o pedido foi criado no sistema.
        /// </summary>
        public DateTime DataPedido { get; set; }

        /// <summary>
        /// Data em que o material foi recebido e conferido.
        /// </summary>
        public DateTime? DataRececao { get; set; }

        /// <summary>
        /// Estado do pedido de material.
        /// </summary>
        public EstadoPedido Estado { get; set; } = EstadoPedido.PENDENTE;

        /// <summary>
        /// Identificador do fornecedor associado ao pedido.
        /// </summary>
        public int Fornecedor_id { get; set; }

        /// <summary>
        /// Navegacao para o fornecedor do pedido.
        /// </summary>
        public Fornecedor? Fornecedor { get; set; }

        /// <summary>
        /// Utilizador que conferiu a rececao do material.
        /// </summary>
        public int? UserConferente_id { get; set; }

        /// <summary>
        /// Navegacao para o utilizador conferente.
        /// </summary>
        public User? Conferente { get; set; }

        /// <summary>
        /// Linhas do pedido associadas a pecas especificas.
        /// </summary>
        public ICollection<ItemPedidoMaterial> Itens { get; set; } = new List<ItemPedidoMaterial>();
    }
}
