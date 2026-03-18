using TipMolde.Core.Interface.ISecurity;
using TipMolde.Core.Interface.IUser;
using TipMolde.Core.Models;

namespace TipMolde.Infrastructure.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasherService passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return _userRepository.GetAllAsync();
        }

        public Task<User?> GetUserByIdAsync(int id)
        {
            return _userRepository.GetByIdAsync(id);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            if (string.IsNullOrWhiteSpace(user.Nome)) throw new ArgumentException("Nome e obrigatorio.");
            if (string.IsNullOrWhiteSpace(user.Email)) throw new ArgumentException("Email e obrigatorio.");
            if (string.IsNullOrWhiteSpace(user.Password)) throw new ArgumentException("Senha e obrigatoria.");

            user.Nome = user.Nome.Trim();
            user.Email = user.Email.Trim().ToLowerInvariant();

            var existing = await _userRepository.GetByEmailAsync(user.Email);
            if (existing is not null) throw new ArgumentException("Ja existe utilizador com este email.");

            user.Password = _passwordHasher.Hash(user.Password);

            await _userRepository.AddAsync(user);
            return user;
        }

        public async Task UpdateUserAsync(User user)
        {
            var existing = await _userRepository.GetByIdAsync(user.User_id);
            if (existing == null)
                throw new KeyNotFoundException($"Utilizador com ID {user.User_id} não encontrado.");

            existing.Nome = string.IsNullOrWhiteSpace(user.Nome) ? existing.Nome : user.Nome.Trim();
            existing.Email = string.IsNullOrWhiteSpace(user.Email) ? existing.Email : user.Email.Trim().ToLowerInvariant();
            existing.Password = string.IsNullOrWhiteSpace(user.Password) ? existing.Password : _passwordHasher.Hash(user.Password.Trim());
            if (user.Role != existing.Role) existing.Role = user.Role;

            await _userRepository.UpdateAsync(existing);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new KeyNotFoundException($"User com ID {id} nao encontrado.");
            }

            await _userRepository.DeleteAsync(id);
        }

        public Task<IEnumerable<User>> SearchByNameAsync(string searchTerm)
        {
            return _userRepository.SearchByNameAsync(searchTerm);
        }

        public Task<User?> GetUserByEmailAsync(string email)
        {
            return _userRepository.GetByEmailAsync(email);
        }
    }
}
