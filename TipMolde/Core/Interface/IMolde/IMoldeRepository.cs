using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IMolde
{
    public interface IMoldeRepository : IGenericRepository<Molde>
    {
        Task<Cliente> GetClienteByIdAsync(int clienteId);
    }
}
