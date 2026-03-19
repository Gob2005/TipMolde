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

        // ────────────────────────── CreateUserAsync ────────────────────────────────────//

        [Fact]
        public async Task CreateUserAsync_ComDadosValidos_CriaUtilizador()
        {
            var user = new User
            {
                User_id = 0,
                Nome = "Gonçalo Barbosa",
                Email = "goncalo@tipmolde.pt",
                Password = "password123",
                Role = UserRole.OPERADOR
            };

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
            var existente = new User
            {
                User_id = 1,
                Nome = "Utilizador Existente",
                Email = "duplicado@tipmolde.pt",
                Password = "hash",
                Role = UserRole.OPERADOR
            };

            _userRepository
                .Setup(r => r.GetByEmailAsync(existente.Email))
                .ReturnsAsync(existente);

            var novoUser = new User
            {
                Nome = "Novo Utilizador",
                Email = "duplicado@tipmolde.pt",
                Password = "password123",
                Role = UserRole.OPERADOR
            };

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _sut.CreateUserAsync(novoUser));
        }

        // ────────────────────────── ChangePasswordAsync ────────────────────────────────────//

        [Fact]
        public async Task ChangePasswordAsync_ComCredenciaisValidas_AtualizaPassword()
        {
            var user = new User
            {
                User_id = 1,
                Nome = "Gonçalo Barbosa",
                Email = "goncalo@tipmolde.pt",
                Password = "hash_da_password_atual",
                Role = UserRole.OPERADOR
            };

            _userRepository
                .Setup(r => r.GetByEmailAsync("goncalo@tipmolde.pt"))
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

            await _sut.ChangePasswordAsync("goncalo@tipmolde.pt", "password_atual", "nova_password");

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
            var user = new User
            {
                User_id = 1,
                Nome = "Teste",
                Email = "teste@email.com",
                Password = "hash_correto",
                Role = UserRole.OPERADOR
            };

            _userRepository
                .Setup(r => r.GetByEmailAsync("teste@email.com"))
                .ReturnsAsync(user);

            _passwordHasher
                .Setup(h => h.IsHash("hash_correto"))
                .Returns(true);

            _passwordHasher
                .Setup(h => h.Verify("password_errada", "hash_correto"))
                .Returns(false);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _sut.ChangePasswordAsync("teste@email.com", "password_errada", "nova_password"));
        }

        // ────────────────────────── ResetPasswordAsync ────────────────────────────────────//

        [Fact]
        public async Task ResetPasswordAsync_ComIdValido_ResetaPassword()
        {
            var user = new User
            {
                User_id = 5,
                Nome = "Utilizador Reset",
                Email = "reset@tipmolde.pt",
                Password = "hash_antigo",
                Role = UserRole.OPERADOR
            };

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
            var user = new User
            {
                User_id = 3,
                Nome = "Operador",
                Email = "op@tipmolde.pt",
                Password = "hash",
                Role = UserRole.OPERADOR
            };

            _userRepository
                .Setup(r => r.GetByIdAsync(3))
                .ReturnsAsync(user);

            _userRepository
                .Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            await _sut.ChangeRoleAsync(3, UserRole.ENGENHEIRO);

            Assert.Equal(UserRole.ENGENHEIRO, user.Role);

            _userRepository.Verify(
                r => r.UpdateAsync(It.Is<User>(u => u.Role == UserRole.ENGENHEIRO)),
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
            var user = new User
            {
                User_id = 7,
                Nome = "A Eliminar",
                Email = "del@tipmolde.pt",
                Password = "hash",
                Role = UserRole.OPERADOR
            };

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
                new() { User_id = 1, Nome = "A", Email = "a@a.pt", Password = "h", Role = UserRole.ADMIN },
                new() { User_id = 2, Nome = "B", Email = "b@b.pt", Password = "h", Role = UserRole.OPERADOR }
            };

            _userRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(lista);

            var resultado = await _sut.GetAllUsersAsync();

            Assert.Equal(2, resultado.Count());
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
        public async Task SearchByNameAsync_ComTermoValido_RetornaResultados()
        {
            var lista = new List<User>
            {
                new() { User_id = 1, Nome = "Gonçalo", Email = "g@g.pt", Password = "h", Role = UserRole.ENGENHEIRO }
            };

            _userRepository
                .Setup(r => r.SearchByNameAsync("Gonçalo"))
                .ReturnsAsync(lista);

            var resultado = await _sut.SearchByNameAsync("Gonçalo");

            Assert.Single(resultado);
        }

        // ────────────────────────── UpdateUserAsync ────────────────────────────────────//

        [Fact]
        public async Task UpdateUserAsync_AlterarSoNome_EmailMantemSe()
        {
            var user = new User
            {
                User_id = 1,
                Nome = "Nome Antigo",
                Email = "antigo@tipmolde.pt",
                Password = "hash",
                Role = UserRole.OPERADOR
            };

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
            var user = new User
            {
                User_id = 1,
                Nome = "Nome Antigo",
                Email = "antigo@tipmolde.pt",
                Password = "hash",
                Role = UserRole.OPERADOR
            };

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
            var user = new User
            {
                User_id = 1,
                Nome = "Nome Antigo",
                Email = "antigo@tipmolde.pt",
                Password = "hash",
                Role = UserRole.OPERADOR
            };

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

            var user = new User
            {
                User_id = 999,
                Nome = "Nao Existe",
                Email = "naoexiste@tipmolde.pt",
                Password = "hash",
                Role = UserRole.OPERADOR
            };

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _sut.UpdateUserAsync(user));
        }

        [Fact]
        public async Task UpdateUserAsync_ComCamposNull_MantemValoresOriginais()
        {
            var user = new User
            {
                User_id = 1,
                Nome = "Nome Original",
                Email = "original@tipmolde.pt",
                Password = "hash",
                Role = UserRole.OPERADOR
            };

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
    }
}
