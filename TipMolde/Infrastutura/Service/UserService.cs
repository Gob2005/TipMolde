using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipMolde.Core.Interface.IUser;
using TipMolde.Core.Models;

namespace TipMolde.Infrastutura.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User> CreateUserAsync(User user)
        {
            if (string.IsNullOrEmpty(user.Nome)) throw new Exception("Nome e obrigatorio.");
            if (string.IsNullOrEmpty(user.Email)) throw new Exception("Email e obrigatorio.");
            if (string.IsNullOrEmpty(user.Password)) throw new Exception("Senha e obrigatoria.");
            if (user.Role == null) throw new Exception("Role e obrigatoria.");
            await _userRepository.AddAsync(user);
            return user;
        }

        public async Task UpdateUserAsync(User user)
        {
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new Exception($"User com ID {id} nao encontrado.");
            }
            await _userRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<User>> SearchByNameAsync(string searchTerm)
        {
            return await _userRepository.SearchByNameAsync(searchTerm);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }
    }
}
