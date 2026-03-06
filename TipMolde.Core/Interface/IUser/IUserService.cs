using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IUser
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(int id);
        Task<IEnumerable<User>> SearchByNameAsync(string searchTerm);
        Task<User?> GetUserByEmailAsync(string email);
    }
}
