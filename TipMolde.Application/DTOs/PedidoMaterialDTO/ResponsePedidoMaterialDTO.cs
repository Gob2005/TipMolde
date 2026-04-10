using TipMolde.Domain.Enums;

namespace TipMolde.Application.DTOs.PedidoMaterialDTO
{
    public class ResponsePedidoMaterialDTO
    {
        public int PedidoMaterialId { get; set; }
        public DateTime DataPedido { get; set; }
        public DateTime? DataRececao { get; set; }
        public EstadoPedido Estado { get; set; }
        public int FornecedorId { get; set; }
        public int? UserConferenteId { get; set; }

        public List<ResponseItemPedidoMaterialDTO> Itens { get; set; } = new();
    }
}
