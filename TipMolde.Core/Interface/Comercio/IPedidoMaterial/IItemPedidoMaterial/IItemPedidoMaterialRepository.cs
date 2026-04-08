using TipMolde.Core.Models.Comercio;

namespace TipMolde.Core.Interface.Comercio.IPedidoMaterial.IItemPedidoMaterial
{
    public interface IItemPedidoMaterialRepository : IGenericRepository<ItemPedidoMaterial>
    {
        Task<IEnumerable<ItemPedidoMaterial>> GetByPedidoIdAsync(int pedidoId);
    }
}
