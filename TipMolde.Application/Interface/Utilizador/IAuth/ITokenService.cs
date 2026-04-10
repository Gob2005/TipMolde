using TipMolde.Domain.Entities;

namespace TipMolde.Application.Interface.Utilizador.IAuth
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
