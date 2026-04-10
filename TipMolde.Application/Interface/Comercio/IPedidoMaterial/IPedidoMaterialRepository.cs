using TipMolde.Domain.Entities.Comercio;

namespace TipMolde.Application.Interface.Comercio.IPedidoMaterial
{
    public interface IPedidoMaterialRepository : IGenericRepository<PedidoMaterial, int>
    {
        Task<PedidoMaterial?> GetWithItensAsync(int id);
        Task<IEnumerable<PedidoMaterial>> GetByFornecedorIdAsync(int fornecedorId);
    }
}
