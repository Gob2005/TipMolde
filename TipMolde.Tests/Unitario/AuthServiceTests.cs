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

    private static User BuildUser(string password = "hash_password") => new()
    {
        User_id = 1,
        Nome = "Operador",
        Email = "teste@tipmolde.pt",
        Password = password,
        Role = UserRole.GESTOR_PRODUCAO
    };

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

    [Test]
    public async Task shouldThrowUnauthorizedWhenUserDoesNotExist()
    {
        _authRepository.Setup(r => r.GetByEmailAsync("naoexiste@tipmolde.pt")).ReturnsAsync((User?)null);

        Func<Task> act = () => _sut.LoginAsync("naoexiste@tipmolde.pt", "Password123!");

        var exception = await act.Should().ThrowAsync<UnauthorizedAccessException>();
        exception.Which.Message.Should().Be("Credenciais invalidas.");
        _tokenService.Verify(t => t.CreateToken(It.IsAny<User>()), Times.Never);
    }

    [Test]
    public async Task shouldThrowUnauthorizedWhenPasswordIsInvalid()
    {
        var user = BuildUser("hash_guardada");
        _authRepository.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.IsHash("hash_guardada")).Returns(true);
        _passwordHasher.Setup(h => h.Verify("Errada123!", "hash_guardada")).Returns(false);

        Func<Task> act = () => _sut.LoginAsync(user.Email, "Errada123!");

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task shouldReturnAuthResponseWhenCredentialsAreValid()
    {
        var user = BuildUser("hash_guardada");
        var token = BuildJwt("jti-login", DateTimeOffset.UtcNow.AddMinutes(30));

        _authRepository.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.IsHash("hash_guardada")).Returns(true);
        _passwordHasher.Setup(h => h.Verify("Correta123!", "hash_guardada")).Returns(true);
        _tokenService.Setup(t => t.CreateToken(user)).Returns(token);

        var result = await _sut.LoginAsync(user.Email, "Correta123!");

        result.Token.Should().Be(token);
        result.ExpiresAt.Should().BeAfter(DateTimeOffset.UtcNow);
    }

    [Test]
    public async Task shouldMigrateLegacyPasswordToHashWhenLegacyPasswordIsValid()
    {
        var user = BuildUser("password_em_texto");
        var token = BuildJwt("jti-legacy", DateTimeOffset.UtcNow.AddMinutes(30));

        _authRepository.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.IsHash("password_em_texto")).Returns(false);
        _passwordHasher.Setup(h => h.Hash("password_em_texto")).Returns("hash_novo");
        _tokenService.Setup(t => t.CreateToken(user)).Returns(token);

        _ = await _sut.LoginAsync(user.Email, "password_em_texto");

        user.Password.Should().Be("hash_novo");
        _authRepository.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Password == "hash_novo")), Times.Once);
    }

    [Test]
    public async Task shouldRevokeTokenWhenLogoutTokenIsValid()
    {
        var token = BuildJwt("jti-logout", DateTimeOffset.UtcNow.AddMinutes(10));

        var result = await _sut.LogoutAsync($"Bearer {token}");

        result.Success.Should().BeTrue();
        _revokedTokenRepository.Verify(r => r.RevokeAsync("jti-logout", It.IsAny<DateTime>()), Times.Once);
    }

    [Test]
    public async Task shouldReturnFailureWhenLogoutTokenIsInvalid()
    {
        var result = await _sut.LogoutAsync("Bearer token_invalido");

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Token invalido.");
        _revokedTokenRepository.Verify(r => r.RevokeAsync(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
    }

    [Test]
    public async Task shouldReturnFailureWhenLogoutTokenIsEmptyOrWhitespace()
    {
        var result = await _sut.LogoutAsync("   ");

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Token ausente.");
        _revokedTokenRepository.Verify(r => r.RevokeAsync(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
    }
}
