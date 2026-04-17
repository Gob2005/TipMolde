using TipMolde.Application.DTOs.AuthDTO;

namespace TipMolde.Application.Interface.Utilizador.IAuth
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> LoginAsync(string email, string password);
        Task<LogoutResultDTO> LogoutAsync(string token);
    }
}
