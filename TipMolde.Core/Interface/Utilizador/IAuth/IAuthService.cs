namespace TipMolde.Core.Interface.Utilizador.IAuth
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string email, string password);
        Task LogoutAsync(string token);
    }
}
