using Microsoft.Extensions.Logging;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Utilizador.ISecurity;
using TipMolde.Application.Interface.Utilizador.IUser;
using TipMolde.Domain.Entities;
using TipMolde.Domain.Enums;

namespace TipMolde.Infrastructure.Service
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly ILogger<UserManagementService> _logger;

        public UserManagementService(IUserRepository userRepository, IPasswordHasherService passwordHasher, ILogger<UserManagementService> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public Task<PagedResult<User>> GetAllAsync(int page = 1, int pageSize = 10) =>
            _userRepository.GetAllAsync(page, pageSize);

        public Task<User?> GetByIdAsync(int id)
        {
            return _userRepository.GetByIdAsync(id);
        }

        public Task<IEnumerable<User>> SearchByNameAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Task.FromResult(Enumerable.Empty<User>());

            return _userRepository.SearchByNameAsync(searchTerm);
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            return _userRepository.GetByEmailAsync(email);
        }

        public async Task<User> CreateAsync(User user)
        {
            _logger.LogInformation("Criacao de utilizador iniciada para email {Email}", user.Email);
            user.Email = user.Email.Trim().ToLowerInvariant();

            var existing = await _userRepository.GetByEmailAsync(user.Email);
            if (existing is not null)
            {
                _logger.LogWarning("Criacao de utilizador falhou: email duplicado {Email}", user.Email);
                throw new ArgumentException("Ja existe utilizador com este email.");
            }

            if (string.IsNullOrWhiteSpace(user.Nome)) throw new ArgumentException("Nome e obrigatorio.");
            if (string.IsNullOrWhiteSpace(user.Email)) throw new ArgumentException("Email e obrigatorio.");
            if (string.IsNullOrWhiteSpace(user.Password)) throw new ArgumentException("Senha e obrigatoria.");

            user.Nome = user.Nome.Trim();
            ValidatePasswordComplexity(user.Password);
            user.Password = _passwordHasher.Hash(user.Password);

            await _userRepository.AddAsync(user);
            _logger.LogInformation("Utilizador criado com sucesso {UserId}", user.User_id);
            return user;
        }

        public async Task UpdateAsync(User user)
        {
            _logger.LogInformation("Atualizacao de utilizador iniciada {UserId}", user.User_id);
            var existing = await _userRepository.GetByIdAsync(user.User_id);
            if (existing == null)
            {
                _logger.LogWarning("Atualizacao falhou: utilizador nao encontrado {UserId}", user.User_id);
                throw new KeyNotFoundException($"Utilizador com ID {user.User_id} não encontrado.");
            }

            existing.Nome = string.IsNullOrWhiteSpace(user.Nome) ? existing.Nome : user.Nome.Trim();
            existing.Email = string.IsNullOrWhiteSpace(user.Email) ? existing.Email : user.Email.Trim().ToLowerInvariant();

            await _userRepository.UpdateAsync(existing);
            _logger.LogInformation("Utilizador atualizado com sucesso {UserId}", user.User_id);
        }

        public async Task ChangeRoleAsync(int userId, UserRole newRole)
        {
            _logger.LogInformation("Alteracao de role iniciada {UserId} -> {Role}", userId, newRole);
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Alteracao de role falhou: utilizador nao encontrado {UserId}", userId);
                throw new KeyNotFoundException($"Utilizador com ID {userId} não encontrado.");
            }

            user.Role = newRole;
            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Role alterada com sucesso {UserId} -> {Role}", userId, newRole);
        }

        public async Task DeleteAsync(int id)
        {
            _logger.LogInformation("Eliminacao de utilizador iniciada {UserId}", id);
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Eliminacao falhou: utilizador nao encontrado {UserId}", id);
                throw new KeyNotFoundException($"User com ID {id} nao encontrado.");
            }

            await _userRepository.DeleteAsync(id);
            _logger.LogInformation("Utilizador eliminado com sucesso {UserId}", id);
        }
        private static void ValidatePasswordComplexity(string password)
        {
            if (password.Length < 8 ||
                !password.Any(char.IsUpper) ||
                !password.Any(char.IsLower) ||
                !password.Any(char.IsDigit) ||
                !password.Any(ch => !char.IsLetterOrDigit(ch)))
                throw new ArgumentException("A password deve ter pelo menos 8 caracteres, maiuscula, minuscula, numero e simbolo.");
        }
    }
}
