using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Interface.Comercio.IPedidoMaterial
{
    public interface IPedidoMaterialService
    {
        Task<PagedResult<PedidoMaterial>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<PedidoMaterial?> GetByIdAsync(int id);
        Task<PedidoMaterial?> GetWithItensAsync(int id);
        Task<IEnumerable<PedidoMaterial>> GetByFornecedorIdAsync(int fornecedorId);

        Task<PedidoMaterial> CreateAsync(PedidoMaterial pedido, IEnumerable<ItemPedidoMaterial> itens);
        Task RegistarRececaoAsync(int pedidoId, int userId);
        Task DeleteAsync(int id);
    }
}
