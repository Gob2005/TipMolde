using Moq;
using TipMolde.Core.Enums;
using TipMolde.Core.Interface.ISecurity;
using TipMolde.Core.Interface.IUser;
using TipMolde.Core.Models;
using TipMolde.Infrastructure.Service;

namespace TipMolde.Tests.Unitario
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IPasswordHasherService> _passwordHasher;

        private readonly UserService _sut;

        public UserServiceTests()
        {
            _userRepository = new Mock<IUserRepository>();
            _passwordHasher = new Mock<IPasswordHasherService>();
            _sut = new UserService(_userRepository.Object, _passwordHasher.Object);
        }

        [Fact]
        public async Task ChangePasswordAsync_ComCredenciaisValidas_AtualizaPassword()
        {
            var user = new User
            {
                User_id = 1,
                Nome = "Teste",
                Email = "teste@email.com",
                Password = "hash_da_password_atual",
                Role = UserRole.OPERADOR
            };

            _userRepository
                .Setup(r => r.GetByEmailAsync("teste@email.com"))
                .ReturnsAsync(user);

            _passwordHasher
                .Setup(h => h.IsHash("hash_da_password_atual"))
                .Returns(true);

            _passwordHasher
                .Setup(h => h.Verify("password_atual", "hash_da_password_atual"))
                .Returns(true);

            _passwordHasher
                .Setup(h => h.Hash("nova_password"))
                .Returns("hash_da_nova_password");

            await _sut.ChangePasswordAsync("teste@email.com", "password_atual", "nova_password");

            _userRepository.Verify(
                r => r.UpdateAsync(It.Is<User>(u => u.Password == "hash_da_nova_password")),
                Times.Once
            );
        }
    }
}
