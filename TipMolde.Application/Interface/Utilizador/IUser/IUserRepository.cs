using TipMolde.Domain.Entities;

namespace TipMolde.Application.Interface.Utilizador.IUser
{
    public interface IUserRepository : IGenericRepository<User, int>
    {
        Task<IEnumerable<User>> SearchByNameAsync(string searchTerm);
        Task<User?> GetByEmailAsync(string email);
    }
}
