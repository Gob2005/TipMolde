using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TipMolde.Application.Interface.Utilizador.ISecurity;
using TipMolde.Application.Interface.Utilizador.IUser;
using TipMolde.Application.Service;
using TipMolde.Domain.Entities;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Unitario.Service;

[TestFixture]
[Category("Unit")]
public class PasswordServiceTests
{
    private Mock<IUserRepository> _userRepository = null!;
    private Mock<IPasswordHasherService> _passwordHasher = null!;
    private Mock<ILogger<PasswordService>> _logger = null!;
    private PasswordService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepository = new Mock<IUserRepository>();
        _passwordHasher = new Mock<IPasswordHasherService>();
        _logger = new Mock<ILogger<PasswordService>>();
        _sut = new PasswordService(_userRepository.Object, _passwordHasher.Object, _logger.Object);
    }

    /// <summary>
    /// Helper para criar entidades User em cenarios de teste de password.
    /// </summary>
    /// <param name="id">Identificador do utilizador.</param>
    /// <param name="password">Password armazenada para o utilizador.</param>
    /// <returns>Instancia de User para composicao de cenarios.</returns>
    private static User BuildUser(int id = 1, string password = "HashAtual") => new()
    {
        User_id = id,
        Nome = "Operador",
        Email = "operador@tipmolde.pt",
        Password = password,
        Role = UserRole.GESTOR_PRODUCAO
    };

    [Test(Description = "T1PWD - ChangePassword deve falhar quando utilizador nao existe.")]
    public async Task ChangePasswordAsync_Should_ThrowKeyNotFoundException_When_UserDoesNotExist()
    {
        // ARRANGE
        _userRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        // ACT
        Func<Task> act = () => _sut.ChangePasswordAsync(99, "OldPass1!", "NovaPass1!");

        // ASSERT
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test(Description = "T2PWD - ChangePassword deve falhar quando password atual e invalida.")]
    public async Task ChangePasswordAsync_Should_ThrowUnauthorizedAccessException_When_CurrentPasswordIsInvalid()
    {
        // ARRANGE
        var user = BuildUser(password: "hash_atual");
        _userRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.IsHash("hash_atual")).Returns(true);
        _passwordHasher.Setup(h => h.Verify("Errada1!", "hash_atual")).Returns(false);

        // ACT
        Func<Task> act = () => _sut.ChangePasswordAsync(1, "Errada1!", "NovaPass1!");

        // ASSERT
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test(Description = "T3PWD - ChangePassword deve aplicar hash e persistir nova password quando password atual e valida.")]
    public async Task ChangePasswordAsync_Should_HashAndPersistNewPassword_When_CurrentPasswordIsValid()
    {
        // ARRANGE
        var user = BuildUser(password: "hash_atual");
        _userRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.IsHash("hash_atual")).Returns(true);
        _passwordHasher.Setup(h => h.Verify("AtualPass1!", "hash_atual")).Returns(true);
        _passwordHasher.Setup(h => h.Hash("NovaPass1!")).Returns("hash_novo");

        // ACT
        await _sut.ChangePasswordAsync(1, "AtualPass1!", "NovaPass1!");

        // ASSERT
        user.Password.Should().Be("hash_novo");
        _userRepository.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Password == "hash_novo")), Times.Once);
    }

    [Test(Description = "T4PWD - ChangePassword deve suportar password legacy em texto simples.")]
    public async Task ChangePasswordAsync_Should_SupportLegacyPlainTextPassword_When_StoredValueIsNotHash()
    {
        // ARRANGE
        var user = BuildUser(password: "AtualPass1!");
        _userRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.IsHash("AtualPass1!")).Returns(false);
        _passwordHasher.Setup(h => h.Hash("NovaPass1!")).Returns("hash_novo");

        // ACT
        await _sut.ChangePasswordAsync(1, "AtualPass1!", "NovaPass1!");

        // ASSERT
        user.Password.Should().Be("hash_novo");
        _userRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Test(Description = "T5PWD - ChangePassword deve falhar quando nova password e fraca.")]
    public async Task ChangePasswordAsync_Should_ThrowArgumentException_When_NewPasswordIsWeak()
    {
        // ARRANGE
        var user = BuildUser(password: "hash_atual");
        _userRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.IsHash("hash_atual")).Returns(true);
        _passwordHasher.Setup(h => h.Verify("AtualPass1!", "hash_atual")).Returns(true);

        // ACT
        Func<Task> act = () => _sut.ChangePasswordAsync(1, "AtualPass1!", "fraca");

        // ASSERT
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Test(Description = "T6PWD - ResetPassword deve falhar quando utilizador nao existe.")]
    public async Task ResetPasswordAsync_Should_ThrowKeyNotFoundException_When_UserDoesNotExist()
    {
        // ARRANGE
        _userRepository.Setup(r => r.GetByIdAsync(77)).ReturnsAsync((User?)null);

        // ACT
        Func<Task> act = () => _sut.ResetPasswordAsync(77, "NovaPass1!");

        // ASSERT
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test(Description = "T7PWD - ResetPassword deve atualizar password com hash quando dados sao validos.")]
    public async Task ResetPasswordAsync_Should_ResetPasswordWithHash_When_InputIsValid()
    {
        // ARRANGE
        var user = BuildUser(id: 3, password: "hash_antigo");
        _userRepository.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Hash("NovaPass1!")).Returns("hash_novo");

        // ACT
        await _sut.ResetPasswordAsync(3, "NovaPass1!");

        // ASSERT
        user.Password.Should().Be("hash_novo");
        _userRepository.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Password == "hash_novo")), Times.Once);
    }
}
