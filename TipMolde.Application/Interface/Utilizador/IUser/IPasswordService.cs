/// <summary>
/// Serviço dedicado à gestão de passwords.
/// Separado do IUserManagementService para respeitar SRP.
/// </summary>
namespace TipMolde.Application.Interface.Utilizador.IUser
{
    public interface IPasswordService
    {
        Task ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task ResetPasswordAsync(int userId, string newPassword);
    }
}
