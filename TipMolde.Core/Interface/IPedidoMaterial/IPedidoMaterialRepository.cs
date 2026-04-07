using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IPedidoMaterial
{
    public interface IPedidoMaterialRepository : IGenericRepository<PedidoMaterial>
    {
        Task<PedidoMaterial?> GetWithItensAsync(int id);
        Task<IEnumerable<PedidoMaterial>> GetByFornecedorIdAsync(int fornecedorId);
    }
}
