namespace TipMolde.Core.Interface.IAuth
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string email, string password);
        Task LogoutAsync(string token);
        Task ChangePasswordAsync(string email, string currentPassword, string newPassword);
    }
}
