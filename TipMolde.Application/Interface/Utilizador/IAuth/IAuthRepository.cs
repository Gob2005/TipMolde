using TipMolde.Domain.Entities;

namespace TipMolde.Application.Interface.Utilizador.IAuth
{
    public interface IAuthRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task UpdateAsync(User user);
    }
}
