using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.Utilizador.IAuth
{
    public interface IAuthRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task UpdateAsync(User user);
    }
}
