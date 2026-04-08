using Moq;
using TipMolde.Core.Enums;
using TipMolde.Core.Interface.Utilizador.IAuth;
using TipMolde.Core.Interface.Utilizador.ISecurity;
using TipMolde.Core.Models;
using TipMolde.Infrastructure.Service;

namespace TipMolde.Tests.Unitario
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _authRepository = new();
        private readonly Mock<IPasswordHasherService> _passwordHasher = new();
        private readonly Mock<ITokenService> _tokenService = new();

        private readonly AuthService _sut;

        public AuthServiceTests()
        {
            _sut = new AuthService(
                _authRepository.Object,
                _passwordHasher.Object,
                _tokenService.Object
            );
        }

        [Fact]
        public async Task LoginAsync_ComCredenciaisValidas_RetornaToken()
        {
            var user = new User
            {
                User_id = 1,
                Nome = "Gonçalo Barbosa",
                Email = "goncalo@tipmolde.pt",
                Password = "hash_da_password",
                Role = UserRole.GESTOR_PRODUCAO
            };

            _authRepository
                .Setup(r => r.GetByEmailAsync("teste@email.com"))
                .ReturnsAsync(user);

            _passwordHasher
                .Setup(h => h.IsHash("hash_da_password"))
                .Returns(true);

            _passwordHasher
                .Setup(h => h.Verify("password123", "hash_da_password"))
                .Returns(true);

            _tokenService
                .Setup(t => t.CreateToken(user))
                .Returns("token_gerado");

            var resultado = await _sut.LoginAsync("teste@email.com", "password123");

            Assert.Equal("token_gerado", resultado);
        }

        [Fact]
        public async Task LoginAsync_ComEmailInexistente_RetornaStringVazia()
        {
            _authRepository
                .Setup(r => r.GetByEmailAsync("naoexiste@email.com"))
                .ReturnsAsync((User?)null);

            var resultado = await _sut.LoginAsync("naoexiste@email.com", "password123");

            Assert.Equal(string.Empty, resultado);
        }

        [Fact]
        public async Task LoginAsync_ComPasswordIncorrecta_RetornaStringVazia()
        {
            var user = new User
            {
                User_id = 1,
                Nome = "Gonçalo Barbosa",
                Email = "goncalo@tipmolde.pt",
                Password = "hash_da_password",
                Role = UserRole.GESTOR_PRODUCAO
            };

            _authRepository
                .Setup(r => r.GetByEmailAsync("teste@email.com"))
                .ReturnsAsync(user);

            _passwordHasher
                .Setup(h => h.IsHash("hash_da_password"))
                .Returns(true);

            _passwordHasher
                .Setup(h => h.Verify("password_errada", "hash_da_password"))
                .Returns(false);

            var resultado = await _sut.LoginAsync("teste@email.com", "password_errada");

            Assert.Equal(string.Empty, resultado);
        }

        [Fact]
        public async Task Logout_Sempre_RetornaCompletedTask()
        {
            var exception = await Record.ExceptionAsync(() =>
                _sut.LogoutAsync("token_qualquer"));

            Assert.Null(exception);
        }
    }
}
