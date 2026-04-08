using TipMolde.Core.Interface.Utilizador.IAuth;
using TipMolde.Core.Interface.Utilizador.ISecurity;

namespace TipMolde.Infrastructure.Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly ITokenService _tokenService;

        public AuthService(
            IAuthRepository authRepository, 
            IPasswordHasherService passwordHasher,
            ITokenService tokenService)
        {
            _authRepository = authRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;

        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _authRepository.GetByEmailAsync(email);
            if (user == null)  return string.Empty;

            bool valid;
            if (_passwordHasher.IsHash(user.Password))
            {
                valid = _passwordHasher.Verify(password, user.Password);
            }
            else
            {
                valid = user.Password == password;
                if (valid)
                {
                    user.Password = _passwordHasher.Hash(password);
                    await _authRepository.UpdateAsync(user);
                }
            }

            if (!valid) return string.Empty;

            return _tokenService.CreateToken(user);
        }

        public Task LogoutAsync(string token)
        {
            return Task.CompletedTask;
        }
    }
}
