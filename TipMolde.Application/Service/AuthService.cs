using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using TipMolde.Application.Dtos.AuthDto;
using TipMolde.Application.Interface.Utilizador.IAuth;
using TipMolde.Application.Interface.Utilizador.ISecurity;

namespace TipMolde.Application.Service
{
    /// <summary>
    /// Implementa os casos de uso de autenticacao de utilizadores.
    /// </summary>
    /// <remarks>
    /// Coordena validacao de credenciais, emissao de JWT e revogacao de tokens em logout.
    /// </remarks>
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IRevokedTokenRepository _revokedTokenRepository;
        private readonly ILogger<AuthService> _logger;

        /// <summary>
        /// Construtor de AuthService.
        /// </summary>
        /// <param name="authRepository">Repositorio para consulta e atualizacao de utilizadores.</param>
        /// <param name="passwordHasher">Servico para validacao e migracao de passwords.</param>
        /// <param name="tokenService">Servico para emissao de tokens JWT.</param>
        /// <param name="revokedTokenRepository">Repositorio para persistencia de tokens revogados.</param>
        /// <param name="logger">Logger para rastreabilidade de eventos de autenticacao.</param>
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

        /// <summary>
        /// Valida credenciais e cria uma sessao autenticada para o utilizador.
        /// </summary>
        /// <remarks>
        /// Fluxo principal:
        /// 1. Procura utilizador por email.
        /// 2. Valida password com hash atual ou fluxo legacy em texto simples.
        /// 3. Migra password legacy para hash quando a validacao e bem-sucedida.
        /// 4. Emite token JWT e devolve instante de expiracao.
        /// </remarks>
        /// <param name="email">Email usado para identificar o utilizador.</param>
        /// <param name="password">Password recebida no pedido de login.</param>
        /// <returns>DTO com token de autenticacao e data de expiracao da sessao.</returns>
        public async Task<AuthResponseDto> LoginAsync(string email, string password)
        {
            var user = await _authRepository.GetByEmailAsync(email);
            _logger.LogInformation("Tentativa de login para email {Email}", email);

            if (user == null)
            {
                _logger.LogWarning("Login falhou para o email {Email}: utilizador nao encontrado", email);
                throw new UnauthorizedAccessException("Credenciais invalidas.");
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
                throw new UnauthorizedAccessException("Credenciais invalidas.");
            }

            var token = _tokenService.CreateToken(user);
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            _logger.LogInformation("Login bem-sucedido para utilizador {UserId}", user.User_id);

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = new DateTimeOffset(DateTime.SpecifyKind(jwt.ValidTo, DateTimeKind.Utc))
            };
        }

        /// <summary>
        /// Revoga um token JWT para encerrar a sessao do utilizador.
        /// </summary>
        /// <remarks>
        /// Fluxo principal:
        /// 1. Valida presenca e formato do token.
        /// 2. Extrai claims obrigatorias jti e exp.
        /// 3. Persiste revogacao ate a expiracao original do token.
        /// </remarks>
        /// <param name="token">Token JWT bruto ou cabecalho Authorization no formato Bearer.</param>
        /// <returns>Resultado funcional com sucesso ou motivo de falha do logout.</returns>
        public async Task<LogoutResultDto> LogoutAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("Logout falhou: token vazio");
                return new LogoutResultDto { Success = false, Message = "Token ausente." };
            }

            var raw = token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                ? token["Bearer ".Length..].Trim()
                : token.Trim();

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(raw))
            {
                _logger.LogWarning("Logout falhou: token ilegivel");
                return new LogoutResultDto { Success = false, Message = "Token invalido." };
            }

            var jwt = handler.ReadJwtToken(raw);
            var jti = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var exp = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;

            if (string.IsNullOrWhiteSpace(jti) || string.IsNullOrWhiteSpace(exp))
            {
                _logger.LogWarning("Logout falhou: token sem claims obrigatorias");
                return new LogoutResultDto { Success = false, Message = "Token sem claims obrigatorias." };
            }

            if (!long.TryParse(exp, out var expUnix))
            {
                _logger.LogWarning("Logout falhou: claim exp invalida para jti {Jti}", jti);
                return new LogoutResultDto { Success = false, Message = "Claim exp invalida." };
            }

            var expiresAt = DateTimeOffset.FromUnixTimeSeconds(expUnix).UtcDateTime;
            await _revokedTokenRepository.RevokeAsync(jti, expiresAt);

            _logger.LogInformation("Logout efetuado. Token revogado com jti {Jti} ate {ExpiresAtUtc}", jti, expiresAt);
            return new LogoutResultDto { Success = true, Message = "Sessao terminada com sucesso." };
        }
    }
}
