using System.IdentityModel.Tokens.Jwt;
using TipMolde.Core.Interface.Utilizador.IAuth;
using TipMolde.Core.Interface.Utilizador.ISecurity;

namespace TipMolde.Infrastructure.Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IRevokedTokenRepository _revokedTokenRepository;

        public AuthService(
            IAuthRepository authRepository, 
            IPasswordHasherService passwordHasher,
            ITokenService tokenService,
            IRevokedTokenRepository revokedTokenRepository)
        {
            _authRepository = authRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _revokedTokenRepository = revokedTokenRepository;
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

        public async Task LogoutAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return;

            var raw = token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? token["Bearer ".Length..].Trim()
                : token.Trim();

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(raw)) return;

            var jwt = handler.ReadJwtToken(raw);
            var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var exp = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;

            if (string.IsNullOrWhiteSpace(jti) || string.IsNullOrWhiteSpace(exp)) return;
            if (!long.TryParse(exp, out var expUnix)) return;

            var expiresAt = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
            await _revokedTokenRepository.RevokeAsync(jti, expiresAt);
        }
    }
}
