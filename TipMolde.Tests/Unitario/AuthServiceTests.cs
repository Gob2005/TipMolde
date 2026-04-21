using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TipMolde.Application.Interface.Utilizador.IAuth;
using TipMolde.Application.Interface.Utilizador.ISecurity;
using TipMolde.Application.Service;
using TipMolde.Domain.Entities;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Unitario;

/// <summary>
/// Testes unitarios do servico de autenticacao.
/// </summary>
/// <remarks>
/// Valida cenarios de login, migracao de credenciais legacy e revogacao de token em logout.
/// </remarks>
[TestFixture]
public class AuthServiceTests
{
    private Mock<IAuthRepository> _authRepository = null!;
    private Mock<IPasswordHasherService> _passwordHasher = null!;
    private Mock<ITokenService> _tokenService = null!;
    private Mock<IRevokedTokenRepository> _revokedTokenRepository = null!;
    private Mock<ILogger<AuthService>> _logger = null!;
    private AuthService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _authRepository = new Mock<IAuthRepository>();
        _passwordHasher = new Mock<IPasswordHasherService>();
        _tokenService = new Mock<ITokenService>();
        _revokedTokenRepository = new Mock<IRevokedTokenRepository>();
        _logger = new Mock<ILogger<AuthService>>();

        _sut = new AuthService(
            _authRepository.Object,
            _passwordHasher.Object,
            _tokenService.Object,
            _revokedTokenRepository.Object,
            _logger.Object);
    }

    /// <summary>
    /// Cria uma entidade de utilizador base para cenarios de teste.
    /// </summary>
    /// <param name="password">Valor inicial de password usado no utilizador.</param>
    /// <returns>Entidade de utilizador pronta para composicao de cenarios.</returns>
    private static User BuildUser(string password = "hash_password") => new()
    {
        User_id = 1,
        Nome = "Operador",
        Email = "teste@tipmolde.pt",
        Password = password,
        Role = UserRole.GESTOR_PRODUCAO
    };

    /// <summary>
    /// Gera um JWT de teste com claims minimas para logout.
    /// </summary>
    /// <param name="jti">Identificador unico do token.</param>
    /// <param name="expiresAtUtc">Data UTC de expiracao do token.</param>
    /// <returns>Token JWT serializado para uso nos testes.</returns>
    private static string BuildJwt(string jti, DateTimeOffset expiresAtUtc)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Jti, jti),
            new(JwtRegisteredClaimNames.Exp, expiresAtUtc.ToUnixTimeSeconds().ToString())
        };

        var token = new JwtSecurityToken(claims: claims, expires: expiresAtUtc.UtcDateTime);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [Test(Description = "T1AUTH - Login deve falhar com nao autorizado quando o utilizador nao existe.")]
    public async Task LoginAsync_Should_ThrowUnauthorized_When_UserDoesNotExist()
    {
        // ARRANGE
        _authRepository.Setup(r => r.GetByEmailAsync("naoexiste@tipmolde.pt")).ReturnsAsync((User?)null);

        // ACT
        Func<Task> act = () => _sut.LoginAsync("naoexiste@tipmolde.pt", "Password123!");

        // ASSERT
        var exception = await act.Should().ThrowAsync<UnauthorizedAccessException>();
        exception.Which.Message.Should().Be("Credenciais invalidas.");
        _tokenService.Verify(t => t.CreateToken(It.IsAny<User>()), Times.Never);
    }

    [Test(Description = "T2AUTH - Login deve falhar com nao autorizado quando a password e invalida.")]
    public async Task LoginAsync_Should_ThrowUnauthorized_When_PasswordIsInvalid()
    {
        // ARRANGE
        var user = BuildUser("hash_guardada");
        _authRepository.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.IsHash("hash_guardada")).Returns(true);
        _passwordHasher.Setup(h => h.Verify("Errada123!", "hash_guardada")).Returns(false);

        // ACT
        Func<Task> act = () => _sut.LoginAsync(user.Email, "Errada123!");

        // ASSERT
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test(Description = "T3AUTH - Login deve devolver token e expiracao quando as credenciais sao validas.")]
    public async Task LoginAsync_Should_ReturnAuthResponse_When_CredentialsAreValid()
    {
        // ARRANGE
        var user = BuildUser("hash_guardada");
        var token = BuildJwt("jti-login", DateTimeOffset.UtcNow.AddMinutes(30));

        _authRepository.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.IsHash("hash_guardada")).Returns(true);
        _passwordHasher.Setup(h => h.Verify("Correta123!", "hash_guardada")).Returns(true);
        _tokenService.Setup(t => t.CreateToken(user)).Returns(token);

        // ACT
        var result = await _sut.LoginAsync(user.Email, "Correta123!");

        // ASSERT
        result.Token.Should().Be(token);
        result.ExpiresAt.Should().BeAfter(DateTimeOffset.UtcNow);
    }

    [Test(Description = "T4AUTH - Login com password legacy valida deve migrar password para hash.")]
    public async Task LoginAsync_Should_MigratePasswordToHash_When_LegacyPasswordIsValid()
    {
        // ARRANGE
        var user = BuildUser("password_em_texto");
        var token = BuildJwt("jti-legacy", DateTimeOffset.UtcNow.AddMinutes(30));

        _authRepository.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.IsHash("password_em_texto")).Returns(false);
        _passwordHasher.Setup(h => h.Hash("password_em_texto")).Returns("hash_novo");
        _tokenService.Setup(t => t.CreateToken(user)).Returns(token);

        // ACT
        _ = await _sut.LoginAsync(user.Email, "password_em_texto");

        // ASSERT
        user.Password.Should().Be("hash_novo");
        _authRepository.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Password == "hash_novo")), Times.Once);
    }

    [Test(Description = "T5AUTH - Logout deve revogar token quando recebe JWT valido.")]
    public async Task LogoutAsync_Should_RevokeToken_When_TokenIsValid()
    {
        // ARRANGE
        var token = BuildJwt("jti-logout", DateTimeOffset.UtcNow.AddMinutes(10));

        // ACT
        var result = await _sut.LogoutAsync($"Bearer {token}");

        // ASSERT
        result.Success.Should().BeTrue();
        _revokedTokenRepository.Verify(r => r.RevokeAsync("jti-logout", It.IsAny<DateTime>()), Times.Once);
    }

    [Test(Description = "T6AUTH - Logout deve devolver falha quando o token e invalido.")]
    public async Task LogoutAsync_Should_ReturnFailure_When_TokenIsInvalid()
    {
        // ARRANGE
        const string invalidToken = "Bearer token_invalido";

        // ACT
        var result = await _sut.LogoutAsync(invalidToken);

        // ASSERT
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Token invalido.");
        _revokedTokenRepository.Verify(r => r.RevokeAsync(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
    }

    [Test(Description = "T7AUTH - Logout deve devolver falha quando o token esta vazio.")]
    public async Task LogoutAsync_Should_ReturnFailure_When_TokenIsEmptyOrWhitespace()
    {
        // ARRANGE
        const string emptyToken = "   ";

        // ACT
        var result = await _sut.LogoutAsync(emptyToken);

        // ASSERT
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Token ausente.");
        _revokedTokenRepository.Verify(r => r.RevokeAsync(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
    }
}
