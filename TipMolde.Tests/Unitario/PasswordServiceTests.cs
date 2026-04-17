using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TipMolde.Application.Interface.Utilizador.ISecurity;
using TipMolde.Application.Interface.Utilizador.IUser;
using TipMolde.Domain.Entities;
using TipMolde.Domain.Enums;
using TipMolde.Application.Service;

namespace TipMolde.Tests.Unitario;

[TestFixture]
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

    private static User BuildUser(int id = 1, string password = "HashAtual") => new()
    {
        User_id = id,
        Nome = "Operador",
        Email = "operador@tipmolde.pt",
        Password = password,
        Role = UserRole.GESTOR_PRODUCAO
    };

    [Test]
    public async Task shouldThrowKeyNotFoundExceptionWhenChangingPasswordForUnknownUser()
    {
        // Arrange
        _userRepository.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((User?)null);

        // Act
        Func<Task> act = () => _sut.ChangePasswordAsync(99, "OldPass1!", "NovaPass1!");

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test]
    public async Task shouldThrowUnauthorizedAccessExceptionWhenCurrentPasswordIsInvalid()
    {
        // Arrange
        var user = BuildUser(password: "hash_atual");
        _userRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.IsHash("hash_atual")).Returns(true);
        _passwordHasher.Setup(h => h.Verify("Errada1!", "hash_atual")).Returns(false);

        // Act
        Func<Task> act = () => _sut.ChangePasswordAsync(1, "Errada1!", "NovaPass1!");

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Test]
    public async Task shouldHashAndPersistNewPasswordWhenCurrentPasswordIsValid()
    {
        // Arrange
        var user = BuildUser(password: "hash_atual");
        _userRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.IsHash("hash_atual")).Returns(true);
        _passwordHasher.Setup(h => h.Verify("AtualPass1!", "hash_atual")).Returns(true);
        _passwordHasher.Setup(h => h.Hash("NovaPass1!")).Returns("hash_novo");

        // Act
        await _sut.ChangePasswordAsync(1, "AtualPass1!", "NovaPass1!");

        // Assert
        user.Password.Should().Be("hash_novo");
        _userRepository.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Password == "hash_novo")), Times.Once);
    }

    [Test]
    public async Task shouldSupportLegacyPlainTextPasswordWhenStoredValueIsNotHash()
    {
        // Arrange
        var user = BuildUser(password: "AtualPass1!");
        _userRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.IsHash("AtualPass1!")).Returns(false);
        _passwordHasher.Setup(h => h.Hash("NovaPass1!")).Returns("hash_novo");

        // Act
        await _sut.ChangePasswordAsync(1, "AtualPass1!", "NovaPass1!");

        // Assert
        user.Password.Should().Be("hash_novo");
        _userRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Test]
    public async Task shouldThrowArgumentExceptionWhenNewPasswordIsWeak()
    {
        // Arrange
        var user = BuildUser(password: "hash_atual");
        _userRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.IsHash("hash_atual")).Returns(true);
        _passwordHasher.Setup(h => h.Verify("AtualPass1!", "hash_atual")).Returns(true);

        // Act
        Func<Task> act = () => _sut.ChangePasswordAsync(1, "AtualPass1!", "fraca");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task shouldThrowKeyNotFoundExceptionWhenResetPasswordForUnknownUser()
    {
        // Arrange
        _userRepository.Setup(r => r.GetByIdAsync(77)).ReturnsAsync((User?)null);

        // Act
        Func<Task> act = () => _sut.ResetPasswordAsync(77, "NovaPass1!");

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test]
    public async Task shouldResetPasswordWithHashWhenInputIsValid()
    {
        // Arrange
        var user = BuildUser(id: 3, password: "hash_antigo");
        _userRepository.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(user);
        _passwordHasher.Setup(h => h.Hash("NovaPass1!")).Returns("hash_novo");

        // Act
        await _sut.ResetPasswordAsync(3, "NovaPass1!");

        // Assert
        user.Password.Should().Be("hash_novo");
        _userRepository.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Password == "hash_novo")), Times.Once);
    }
}
