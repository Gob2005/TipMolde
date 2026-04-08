using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.Utilizador.IAuth
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
