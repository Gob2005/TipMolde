using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace TipMolde.Tests.Integracao;

/// <summary>
/// Autenticacao controlada para testes de integracao da API.
/// </summary>
/// <remarks>
/// Permite validar endpoints protegidos sem depender da emissao real de JWT.
/// A ausencia do header Authorization mantem o pedido nao autenticado.
/// </remarks>
public sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "Test";
    public const string AuthorizationValue = "Test";
    public const string UserIdHeader = "X-Test-UserId";
    public const string RolesHeader = "X-Test-Roles";
    public const string MissingUserId = "__missing_user_id__";

    /// <summary>
    /// Construtor de TestAuthHandler.
    /// </summary>
    /// <param name="options">Opcoes de autenticacao usadas pelo pipeline ASP.NET Core.</param>
    /// <param name="logger">Factory de logs do handler.</param>
    /// <param name="encoder">Codificador de URL usado pela classe base.</param>
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    /// <summary>
    /// Cria uma identidade autenticada a partir dos headers de teste.
    /// </summary>
    /// <returns>Resultado de autenticacao usado pelo pipeline HTTP de teste.</returns>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authorizationHeader)
            || !authorizationHeader.ToString().StartsWith(AuthorizationValue, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var userId = Request.Headers.TryGetValue(UserIdHeader, out var userIdHeader)
            ? userIdHeader.ToString()
            : "1";

        var roles = Request.Headers.TryGetValue(RolesHeader, out var rolesHeader)
            ? rolesHeader.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            : new[] { "ADMIN" };

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "Utilizador Teste")
        };

        if (!string.Equals(userId, MissingUserId, StringComparison.Ordinal))
        {
            claims.Add(new Claim("sub", userId));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userId));
        }

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
