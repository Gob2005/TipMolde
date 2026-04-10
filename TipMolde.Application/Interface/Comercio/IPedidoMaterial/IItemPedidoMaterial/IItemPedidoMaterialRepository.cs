using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Interface.Comercio.IPedidoMaterial.IItemPedidoMaterial
{
    public interface IItemPedidoMaterialRepository : IGenericRepository<ItemPedidoMaterial, int>
    {
        Task<IEnumerable<ItemPedidoMaterial>> GetByPedidoIdAsync(int pedidoId);
    }
}
