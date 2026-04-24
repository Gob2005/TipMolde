using Microsoft.Extensions.Logging;
using AutoMapper;
using TipMolde.Application.Dtos.UserDto;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Utilizador.ISecurity;
using TipMolde.Application.Interface.Utilizador.IUser;
using TipMolde.Domain.Entities;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.Service
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly IMapper _mapper;
        private readonly ILogger<UserManagementService> _logger;

        public UserManagementService(
            IUserRepository userRepository,
            IPasswordHasherService passwordHasher,
            IMapper mapper,
            ILogger<UserManagementService> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PagedResult<ResponseUserDto>> GetAllAsync(int page = 1, int pageSize = 10)
        {
            var result = await _userRepository.GetAllAsync(page, pageSize);
            var mappedItems = _mapper.Map<IEnumerable<ResponseUserDto>>(result.Items);
            return new PagedResult<ResponseUserDto>(mappedItems, result.TotalCount, result.CurrentPage, result.PageSize);
        }

        public async Task<ResponseUserDto?> GetByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : _mapper.Map<ResponseUserDto>(user);
        }

        public async Task<PagedResult<ResponseUserDto>> SearchByNameAsync(string searchTerm, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new PagedResult<ResponseUserDto>(Enumerable.Empty<ResponseUserDto>(), 0, page, pageSize);

            var users = await _userRepository.SearchByNameAsync(searchTerm);
            var normalizedPage = page < 1 ? 1 : page;
            var normalizedPageSize = pageSize < 1 ? 10 : pageSize > 200 ? 200 : pageSize;
            var totalCount = users.Count();
            var pagedItems = users
                .Skip((normalizedPage - 1) * normalizedPageSize)
                .Take(normalizedPageSize);
            var mappedItems = _mapper.Map<IEnumerable<ResponseUserDto>>(pagedItems);

            return new PagedResult<ResponseUserDto>(mappedItems, totalCount, normalizedPage, normalizedPageSize);
        }

        public async Task<ResponseUserDto?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user == null ? null : _mapper.Map<ResponseUserDto>(user);
        }

        public async Task<ResponseUserDto> CreateAsync(CreateUserDto dto)
        {
            var user = _mapper.Map<User>(dto);
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
            return _mapper.Map<ResponseUserDto>(user);
        }

        public async Task UpdateAsync(int id, UpdateUserDto dto)
        {
            _logger.LogInformation("Atualizacao de utilizador iniciada {UserId}", id);
            var existing = await _userRepository.GetByIdAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("Atualizacao falhou: utilizador nao encontrado {UserId}", id);
                throw new KeyNotFoundException($"Utilizador com ID {id} não encontrado.");
            }

            var hasChanges =
                !string.IsNullOrWhiteSpace(dto.Nome) ||
                !string.IsNullOrWhiteSpace(dto.Email);

            if (!hasChanges)
                throw new ArgumentException("Pelo menos um campo deve ser informado para atualizacao.");

            _mapper.Map(dto, existing);

            if (!string.IsNullOrWhiteSpace(dto.Email))
                existing.Email = existing.Email.Trim().ToLowerInvariant();

            await _userRepository.UpdateAsync(existing);
            _logger.LogInformation("Utilizador atualizado com sucesso {UserId}", id);
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
