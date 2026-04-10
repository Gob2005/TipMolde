using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using TipMolde.Application.Interface.Utilizador.IAuth;
using TipMolde.Application.Interface.Utilizador.ISecurity;

namespace TipMolde.Infrastructure.Service
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IRevokedTokenRepository _revokedTokenRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IAuthRepository authRepository, 
            IPasswordHasherService passwordHasher,
            ITokenService tokenService,
            IRevokedTokenRepository revokedTokenRepository,
            ILogger<AuthService> logger)
        {
            _authRepository = authRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _revokedTokenRepository = revokedTokenRepository;
            _logger = logger;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var user = await _authRepository.GetByEmailAsync(email);
            _logger.LogInformation("Tentativa de login para email {Email}", email);
            if (user == null) 
            {
                _logger.LogWarning("Login falhou para o email {Email}: utilizador não encontrado", email);
                return string.Empty;
            }

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
                    _logger.LogInformation("Password migrada para hash para utilizador {UserId}", user.User_id);
                }
            }

            if (!valid)
            {
                _logger.LogWarning("Login falhou: password invalida para utilizador {UserId}", user.User_id);
                return string.Empty;
            }

            _logger.LogInformation("Login bem-sucedido para utilizador {UserId}", user.User_id);
            return _tokenService.CreateToken(user);
        }

        public async Task LogoutAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("Logout ignorado: token vazio");
                return;
            }

            var raw = token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? token["Bearer ".Length..].Trim()
                : token.Trim();

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(raw))
            {
                _logger.LogWarning("Logout ignorado: token ilegivel");
                return;
            }

            var jwt = handler.ReadJwtToken(raw);
            var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var exp = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;

            if (string.IsNullOrWhiteSpace(jti) || string.IsNullOrWhiteSpace(exp))
            {
                _logger.LogWarning("Logout ignorado: token sem claims obrigatorias");
                return;
            }
            if (!long.TryParse(exp, out var expUnix))
            {
                _logger.LogWarning("Logout ignorado: claim exp invalida para jti {Jti}", jti);
                return;
            }

            var expiresAt = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
            _logger.LogInformation("Logout efetuado. Token revogado com jti {Jti} ate {ExpiresAtUtc}", jti, expiresAt);
            await _revokedTokenRepository.RevokeAsync(jti, expiresAt);
        }
    }
}
