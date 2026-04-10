using Microsoft.EntityFrameworkCore;
using TipMolde.Application.Interface.Utilizador.IUser;
using TipMolde.Domain.Entities;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class UserRepository : GenericRepository<User, int>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<User>> SearchByNameAsync(string searchTerm)
        {
            var term = searchTerm.Trim().ToLower();
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Nome.ToLower().Contains(term))
                .OrderBy(u => u.Nome)
                .ToListAsync();
        }

        public Task<User?> GetByEmailAsync(string email) =>
            _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
    }
}
