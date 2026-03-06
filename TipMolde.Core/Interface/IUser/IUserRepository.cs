using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IUser
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<IEnumerable<User>> SearchByNameAsync(string searchTerm);

        Task<User?> GetByEmailAsync(string email);
    }
}
