using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IPedidoMaterial.IItemPedidoMaterial
{
    public interface IItemPedidoMaterialRepository : IGenericRepository<ItemPedidoMaterial>
    {
        Task<IEnumerable<ItemPedidoMaterial>> GetByPedidoIdAsync(int pedidoId);
    }
}
