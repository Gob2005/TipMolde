using Moq;
using TipMolde.Core.Enums;
using TipMolde.Core.Interface.Utilizador.ISecurity;
using TipMolde.Core.Interface.Utilizador.IUser;
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

        private static User UserFake(
            int id = 1,
            string nome = "Gonçalo Barbosa",
            string email = "goncalo@tipmolde.pt",
            string password = "hash_da_password",
            UserRole role = UserRole.GESTOR_PRODUCAO) => new()
            {
                User_id = id,
                Nome = nome,
                Email = email,
                Password = password,
                Role = role
            };

        // ────────────────────────── CreateUserAsync ────────────────────────────────────//

        [Fact]
        public async Task CreateUserAsync_ComDadosValidos_CriaUtilizador()
        {
            var user = UserFake(id: 0, password: "password123");

            _userRepository
                .Setup(r => r.GetByEmailAsync(user.Email))
                .ReturnsAsync((User?)null);

            _passwordHasher
                .Setup(h => h.Hash(user.Password))
                .Returns("hash_gerado");

            _userRepository
                .Setup(r => r.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            var resultado = await _sut.CreateUserAsync(user);

            Assert.NotNull(resultado);
            Assert.Equal(user.Nome, resultado.Nome);
            Assert.Equal(user.Email, resultado.Email);
            Assert.Equal(user.Role, resultado.Role);
            Assert.Equal("hash_gerado", resultado.Password);
            _userRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_ComEmailDuplicado_LancaExcecao()
        {
            var existente = UserFake(id: 1, email: "duplicado@tipmolde.pt");

            _userRepository
                .Setup(r => r.GetByEmailAsync(existente.Email))
                .ReturnsAsync(existente);

            var novoUser = UserFake(id: 0, nome: "Novo Utilizador", email: "duplicado@tipmolde.pt", password: "password123");

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.CreateUserAsync(novoUser));
        }

        [Fact]
        public async Task CreateUserAsync_ComNomeVazio_LancaExcecao()
        {
            var user = UserFake(id: 0, nome: "", password: "password123");

            _userRepository
                .Setup(r => r.GetByEmailAsync(user.Email))
                .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.CreateUserAsync(user));
        }

        [Fact]
        public async Task CreateUserAsync_ComEmailVazio_LancaExcecao()
        {
            var user = UserFake(id: 0, email: "", password: "password123");

            _userRepository
                .Setup(r => r.GetByEmailAsync(user.Email))
                .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.CreateUserAsync(user));
        }

        [Fact]
        public async Task CreateUserAsync_ComPasswordVazia_LancaExcecao()
        {
            var user = UserFake(id: 0, password: "");

            _userRepository
                .Setup(r => r.GetByEmailAsync(user.Email))
                .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.CreateUserAsync(user));
        }

        // ────────────────────────── UpdateUserAsync ────────────────────────────────────//

        [Fact]
        public async Task UpdateUserAsync_AlterarSoNome_EmailMantemSe()
        {
            var user = UserFake(id: 1, nome: "Nome Antigo", email: "antigo@tipmolde.pt");

            _userRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            _userRepository
                .Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            user.Nome = "Nome Novo";

            await _sut.UpdateUserAsync(user);

            _userRepository.Verify(
                r => r.UpdateAsync(It.Is<User>(u =>
                    u.Nome == "Nome Novo" &&
                    u.Email == "antigo@tipmolde.pt")),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateUserAsync_AlterarSoEmail_NomeMantemSe()
        {
            var user = UserFake(id: 1, nome: "Nome Antigo", email: "antigo@tipmolde.pt");

            _userRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            _userRepository
                .Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            user.Email = "novo@tipmolde.pt";

            await _sut.UpdateUserAsync(user);

            _userRepository.Verify(
                r => r.UpdateAsync(It.Is<User>(u =>
                    u.Nome == "Nome Antigo" &&
                    u.Email == "novo@tipmolde.pt")),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateUserAsync_AlterarNomeEEmail_AmbosMudam()
        {
            var user = UserFake(id: 1, nome: "Nome Antigo", email: "antigo@tipmolde.pt");

            _userRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            _userRepository
                .Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            user.Nome = "Nome Novo";
            user.Email = "novo@tipmolde.pt";

            await _sut.UpdateUserAsync(user);

            _userRepository.Verify(
                r => r.UpdateAsync(It.Is<User>(u =>
                    u.Nome == "Nome Novo" &&
                    u.Email == "novo@tipmolde.pt")),
                Times.Once
            );
        }

        [Fact]
        public async Task UpdateUserAsync_ComIdInexistente_LancaExcecao()
        {
            _userRepository
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((User?)null);

            var user = UserFake(id: 999, nome: "Nao Existe", email: "naoexiste@tipmolde.pt");

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.UpdateUserAsync(user));
        }

        [Fact]
        public async Task UpdateUserAsync_ComCamposNull_MantemValoresOriginais()
        {
            var user = UserFake(id: 1, nome: "Nome Original", email: "original@tipmolde.pt");

            _userRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            _userRepository
                .Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            await _sut.UpdateUserAsync(user);

            _userRepository.Verify(
                r => r.UpdateAsync(It.Is<User>(u =>
                    u.Nome == "Nome Original" &&
                    u.Email == "original@tipmolde.pt")),
                Times.Once
            );
        }

        // ────────────────────────── ChangePasswordAsync ────────────────────────────────────//

        [Fact]
        public async Task ChangePasswordAsync_ComCredenciaisValidas_AtualizaPassword()
        {
            var user = UserFake(password: "hash_da_password_atual");

            _userRepository
                .Setup(r => r.GetByEmailAsync(user.Email))
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

            await _sut.ChangePasswordAsync(user.Email, "password_atual", "nova_password");

            Assert.Equal("hash_da_nova_password", user.Password);

            _userRepository.Verify(
                r => r.UpdateAsync(It.Is<User>(u => u.Password == "hash_da_nova_password")),
                Times.Once
            );
        }

        [Fact]
        public async Task ChangePasswordAsync_ComEmailInexistente_LancaExcecao()
        {
            _userRepository
                .Setup(r => r.GetByEmailAsync("naoexiste@email.com"))
                .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.ChangePasswordAsync("naoexiste@email.com", "pass", "novaPass"));
        }

        [Fact]
        public async Task ChangePasswordAsync_ComPasswordAtualIncorreta_LancaExcecao()
        {
            var user = UserFake(password: "hash_correto");

            _userRepository
                .Setup(r => r.GetByEmailAsync(user.Email))
                .ReturnsAsync(user);

            _passwordHasher
                .Setup(h => h.IsHash("hash_correto"))
                .Returns(true);

            _passwordHasher
                .Setup(h => h.Verify("password_errada", "hash_correto"))
                .Returns(false);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _sut.ChangePasswordAsync(user.Email, "password_errada", "nova_password"));
        }

        // ────────────────────────── ResetPasswordAsync ────────────────────────────────────//

        [Fact]
        public async Task ResetPasswordAsync_ComIdValido_ResetaPassword()
        {
            var user = UserFake(id: 5, email: "reset@tipmolde.pt", password: "hash_antigo");

            _userRepository
                .Setup(r => r.GetByIdAsync(5))
                .ReturnsAsync(user);

            _passwordHasher
                .Setup(h => h.Hash("novaPasswordAdmin"))
                .Returns("hash_novo");

            _userRepository
                .Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            await _sut.ResetPasswordAsync(5, "novaPasswordAdmin");

            Assert.Equal("hash_novo", user.Password);

            _userRepository.Verify(
                r => r.UpdateAsync(It.Is<User>(u => u.Password == "hash_novo")),
                Times.Once
            );
        }

        [Fact]
        public async Task ResetPasswordAsync_ComIdInexistente_LancaExcecao()
        {
            _userRepository
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.ResetPasswordAsync(999, "novaPassword"));
        }

        // ────────────────────────── ChangeRoleAsync ────────────────────────────────────//

        [Fact]
        public async Task ChangeRoleAsync_ComIdValido_AlteraRole()
        {
            var user = UserFake(id: 3, email: "op@tipmolde.pt");

            _userRepository
                .Setup(r => r.GetByIdAsync(3))
                .ReturnsAsync(user);

            _userRepository
                .Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            await _sut.ChangeRoleAsync(3, UserRole.GESTOR_DESENHO);

            Assert.Equal(UserRole.GESTOR_DESENHO, user.Role);

            _userRepository.Verify(
                r => r.UpdateAsync(It.Is<User>(u => u.Role == UserRole.GESTOR_DESENHO)),
                Times.Once
            );
        }

        [Fact]
        public async Task ChangeRoleAsync_ComIdInexistente_LancaExcecao()
        {
            _userRepository
                .Setup(r => r.GetByIdAsync(404))
                .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.ChangeRoleAsync(404, UserRole.ADMIN));
        }

        // ────────────────────────── DeleteUserAsync ────────────────────────────────────//

        [Fact]
        public async Task DeleteUserAsync_ComIdValido_EliminaUtilizador()
        {
            var user = UserFake(id: 7, email: "del@tipmolde.pt");

            _userRepository
                .Setup(r => r.GetByIdAsync(7))
                .ReturnsAsync(user);

            _userRepository
                .Setup(r => r.DeleteAsync(7))
                .Returns(Task.CompletedTask);

            await _sut.DeleteUserAsync(7);

            _userRepository.Verify(r => r.DeleteAsync(7), Times.Once);
        }

        [Fact]
        public async Task DeleteUserAsync_ComIdInexistente_LancaExcecao()
        {
            _userRepository
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.DeleteUserAsync(999));
        }

        // ─── GetAllUsersAsync / GetUserByIdAsync / SearchByNameAsync ─────── //

        [Fact]
        public async Task GetAllUsersAsync_RetornaListaDeUtilizadores()
        {
            var lista = new List<User>
            {
                UserFake(id: 1, email: "a@a.pt", role: UserRole.ADMIN),
                UserFake(id: 2, nome: "B", email: "b@b.pt")
            };

            _userRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(lista);

            var resultado = await _sut.GetAllUsersAsync();

            Assert.Equal(2, resultado.Count());
        }

        [Fact]
        public async Task GetAllUsersAsync_SemUtilizadores_RetornaListaVazia()
        {
            _userRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<User>());

            var resultado = await _sut.GetAllUsersAsync();

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task GetUserByIdAsync_ComIdValido_RetornaUtilizador()
        {
            var user = UserFake(id: 1);

            _userRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(user);

            var resultado = await _sut.GetUserByIdAsync(1);

            Assert.NotNull(resultado);
            Assert.Equal(1, resultado!.User_id);
        }

        [Fact]
        public async Task GetUserByIdAsync_ComIdInexistente_RetornaNull()
        {
            _userRepository
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((User?)null);

            var resultado = await _sut.GetUserByIdAsync(999);

            Assert.Null(resultado);
        }

        [Fact]
        public async Task SearchByNameAsync_ComTermoSemResultados_RetornaListaVazia()
        {
            _userRepository
                .Setup(r => r.SearchByNameAsync("TermoInexistente"))
                .ReturnsAsync(new List<User>());

            var resultado = await _sut.SearchByNameAsync("TermoInexistente");

            Assert.Empty(resultado);
        }

        [Fact]
        public async Task SearchByNameAsync_ComTermoValido_RetornaResultados()
        {
            var lista = new List<User>
            {
                UserFake(id: 1, email: "g@g.pt", role: UserRole.GESTOR_DESENHO)
            };

            _userRepository
                .Setup(r => r.SearchByNameAsync("Gonçalo Barbosa"))
                .ReturnsAsync(lista);

            var resultado = await _sut.SearchByNameAsync("Gonçalo Barbosa");

            Assert.Single(resultado);
        }
    }
}