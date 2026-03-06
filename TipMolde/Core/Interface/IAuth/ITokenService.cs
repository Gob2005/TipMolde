using TipMolde.Core.Models;

namespace TipMolde.Core.Interface.IAuth
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
