using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Utilizador.ISecurity;
using TipMolde.Application.Interface.Utilizador.IUser;
using TipMolde.Domain.Entities;
using TipMolde.Domain.Enums;
using TipMolde.Application.Service;

namespace TipMolde.Tests.Unitario;

[TestFixture]
public class UserManagementServiceTests
{
    private Mock<IUserRepository> _userRepository = null!;
    private Mock<IPasswordHasherService> _passwordHasher = null!;
    private Mock<ILogger<UserManagementService>> _logger = null!;
    private UserManagementService _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepository = new Mock<IUserRepository>();
        _passwordHasher = new Mock<IPasswordHasherService>();
        _logger = new Mock<ILogger<UserManagementService>>();
        _sut = new UserManagementService(_userRepository.Object, _passwordHasher.Object, _logger.Object);
    }

    private static User BuildUser(int id = 1, string nome = "Operador", string email = "operador@tipmolde.pt", string password = "Passw0rd!") => new()
    {
        User_id = id,
        Nome = nome,
        Email = email,
        Password = password,
        Role = UserRole.GESTOR_PRODUCAO
    };

    [Test]
    public async Task shouldReturnAllUsersWhenGettingAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            BuildUser(id: 1, nome: "Ana", email: "ana@tipmolde.pt"),
            BuildUser(id: 2, nome: "Bruno", email: "bruno@tipmolde.pt")
        };

        var pagedResult = new PagedResult<User>(users, users.Count, 1, 10);
        _userRepository.Setup(r => r.GetAllAsync(1, 10)).ReturnsAsync(pagedResult);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
        result.CurrentPage.Should().Be(1);
        result.PageSize.Should().Be(10);
        _userRepository.Verify(r => r.GetAllAsync(1, 10), Times.Once);
    }

    [Test]
    public async Task shouldThrowArgumentExceptionWhenCreatingUserWithDuplicateEmail()
    {
        // Arrange
        var user = BuildUser(email: "duplicado@tipmolde.pt");
        _userRepository.Setup(r => r.GetByEmailAsync("duplicado@tipmolde.pt")).ReturnsAsync(BuildUser(id: 2));

        // Act
        Func<Task> act = () => _sut.CreateAsync(user);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task shouldThrowArgumentExceptionWhenCreatingUserWithWeakPassword()
    {
        // Arrange
        var user = BuildUser(password: "fraca");
        _userRepository.Setup(r => r.GetByEmailAsync("operador@tipmolde.pt")).ReturnsAsync((User?)null);

        // Act
        Func<Task> act = () => _sut.CreateAsync(user);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Test]
    public async Task shouldTrimNormalizeAndHashWhenCreatingValidUser()
    {
        // Arrange
        var user = BuildUser(nome: "  Operador  ", email: "  OP@TipMolde.PT  ", password: "Valida123!");
        _userRepository.Setup(r => r.GetByEmailAsync("op@tipmolde.pt")).ReturnsAsync((User?)null);
        _passwordHasher.Setup(h => h.Hash("Valida123!")).Returns("hash_gerado");

        // Act
        var result = await _sut.CreateAsync(user);

        // Assert
        result.Nome.Should().Be("Operador");
        result.Email.Should().Be("op@tipmolde.pt");
        result.Password.Should().Be("hash_gerado");
        _userRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Test]
    public async Task shouldThrowKeyNotFoundExceptionWhenUpdatingUnknownUser()
    {
        // Arrange
        _userRepository.Setup(r => r.GetByIdAsync(404)).ReturnsAsync((User?)null);

        // Act
        Func<Task> act = () => _sut.UpdateAsync(BuildUser(id: 404));

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test]
    public async Task shouldUpdateOnlyProvidedFieldsWhenUpdatingUser()
    {
        // Arrange
        var existing = BuildUser(id: 5, nome: "Nome Antigo", email: "antigo@tipmolde.pt");
        _userRepository.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(existing);

        var update = BuildUser(id: 5, nome: "  Nome Novo  ", email: "  NOVO@TipMolde.PT ");

        // Act
        await _sut.UpdateAsync(update);

        // Assert
        _userRepository.Verify(r => r.UpdateAsync(It.Is<User>(u =>
            u.User_id == 5 &&
            u.Nome == "Nome Novo" &&
            u.Email == "novo@tipmolde.pt")), Times.Once);
    }

    [Test]
    public async Task shouldThrowKeyNotFoundExceptionWhenChangingRoleForUnknownUser()
    {
        // Arrange
        _userRepository.Setup(r => r.GetByIdAsync(777)).ReturnsAsync((User?)null);

        // Act
        Func<Task> act = () => _sut.ChangeRoleAsync(777, UserRole.ADMIN);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test]
    public async Task shouldChangeRoleWhenUserExists()
    {
        // Arrange
        var user = BuildUser(id: 3);
        _userRepository.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(user);

        // Act
        await _sut.ChangeRoleAsync(3, UserRole.ADMIN);

        // Assert
        user.Role.Should().Be(UserRole.ADMIN);
        _userRepository.Verify(r => r.UpdateAsync(It.Is<User>(u => u.Role == UserRole.ADMIN)), Times.Once);
    }

    [Test]
    public async Task shouldThrowKeyNotFoundExceptionWhenDeletingUnknownUser()
    {
        // Arrange
        _userRepository.Setup(r => r.GetByIdAsync(1000)).ReturnsAsync((User?)null);

        // Act
        Func<Task> act = () => _sut.DeleteAsync(1000);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Test]
    public async Task shouldDeleteUserWhenUserExists()
    {
        // Arrange
        _userRepository.Setup(r => r.GetByIdAsync(7)).ReturnsAsync(BuildUser(id: 7));

        // Act
        await _sut.DeleteAsync(7);

        // Assert
        _userRepository.Verify(r => r.DeleteAsync(7), Times.Once);
    }
}
