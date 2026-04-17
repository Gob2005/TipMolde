using TipMolde.Domain.Entities;
using TipMolde.Domain.Enums;

/// <summary>
/// Serviço de gestão de utilizadores.
/// Responsável exclusivamente por operações CRUD e pesquisa de utilizadores.
/// </summary>
/// <remarks>
/// Operações relacionadas com passwords foram movidas para IPasswordService
/// para respeitar o Princípio da Responsabilidade Única.
/// </remarks>
namespace TipMolde.Application.Interface.Utilizador.IUser
{
    public interface IUserManagementService
    {
        Task<PagedResult<User>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> SearchByNameAsync(string searchTerm);
        Task<User> CreateAsync(User user);
        Task UpdateAsync(User user);
        Task ChangeRoleAsync(int id, UserRole newRole);
        Task DeleteAsync(int id);
    }
}