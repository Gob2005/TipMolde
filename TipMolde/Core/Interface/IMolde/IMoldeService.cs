using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IMolde
{
    public interface IMoldeService
    {
        Task<IEnumerable<Molde>> GetAllMoldesAsync();
        Task<Molde> GetMoldeByIdAsync(int id);
        Task<Molde> CreateMoldeAsync(Molde molde);
        Task UpdateMoldeAsync(Molde molde);
        Task DeleteMoldeAsync(int id);
        Task<Cliente> GetClienteByIdAsync(int clienteId);
    }
}
