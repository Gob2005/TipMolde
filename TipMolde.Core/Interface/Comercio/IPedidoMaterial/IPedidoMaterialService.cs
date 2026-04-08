using TipMolde.Core.Models.Comercio;

namespace TipMolde.Core.Interface.Comercio.IPedidoMaterial
{
    public interface IPedidoMaterialService
    {
        Task<IEnumerable<PedidoMaterial>> GetAllAsync();
        Task<PedidoMaterial?> GetByIdAsync(int id);
        Task<PedidoMaterial?> GetWithItensAsync(int id);
        Task<IEnumerable<PedidoMaterial>> GetByFornecedorIdAsync(int fornecedorId);

        Task<PedidoMaterial> CreateAsync(PedidoMaterial pedido, IEnumerable<ItemPedidoMaterial> itens);
        Task RegistarRececaoAsync(int pedidoId, int userId);
        Task DeleteAsync(int id);
    }
}
