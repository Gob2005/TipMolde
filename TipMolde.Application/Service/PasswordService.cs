using Microsoft.Extensions.Logging;
using TipMolde.Application.Interface.Utilizador.ISecurity;
using TipMolde.Application.Interface.Utilizador.IUser;

namespace TipMolde.Infrastructure.Service
{
    public class PasswordService : IPasswordService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly ILogger<PasswordService> _logger;

        public PasswordService(IUserRepository userRepository, IPasswordHasherService passwordHasher, ILogger<PasswordService> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            _logger.LogInformation("Alteracao de password iniciada para utilizador {UserId}", userId);
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Alteracao de password falhou: utilizador nao encontrado {UserId}", userId);
                throw new KeyNotFoundException("Utilizador nao encontrado.");
            }

            bool valid = _passwordHasher.IsHash(user.Password)
                ? _passwordHasher.Verify(currentPassword, user.Password)
                : user.Password == currentPassword;

            if (!valid)
            {
                _logger.LogWarning("Alteracao de password falhou: password atual invalida para utilizador {UserId}", userId);
                throw new UnauthorizedAccessException("Password atual invalida.");
            }

            ValidatePasswordComplexity(newPassword);
            user.Password = _passwordHasher.Hash(newPassword);
            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Password alterada com sucesso para utilizador {UserId}", userId);
        }


        public async Task ResetPasswordAsync(int userId, string newPassword)
        {
            _logger.LogInformation("Reset de password iniciado para utilizador {UserId}", userId);
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Reset de password falhou: utilizador nao encontrado {UserId}", userId);
                throw new KeyNotFoundException($"Utilizador com ID {userId} nao encontrado.");
            }

            ValidatePasswordComplexity(newPassword);
            user.Password = _passwordHasher.Hash(newPassword);
            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Reset de password concluido para utilizador {UserId}", userId);
        }

        private static void ValidatePasswordComplexity(string password)
        {
            if (password.Length < 8 ||
                !password.Any(char.IsUpper) ||
                !password.Any(char.IsLower) ||
                !password.Any(char.IsDigit) ||
                !password.Any(ch => !char.IsLetterOrDigit(ch)))
                throw new ArgumentException("A password deve ter pelo menos 8 caracteres, maiuscula, minuscula, numero e simbolo.");
        }
    }
}
