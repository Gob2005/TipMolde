using TipMolde.Core.Models.Comercio;

namespace TipMolde.Core.Interface.Comercio.IPedidoMaterial
{
    public interface IPedidoMaterialRepository : IGenericRepository<PedidoMaterial>
    {
        Task<PedidoMaterial?> GetWithItensAsync(int id);
        Task<IEnumerable<PedidoMaterial>> GetByFornecedorIdAsync(int fornecedorId);
    }
}
