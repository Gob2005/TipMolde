using Microsoft.EntityFrameworkCore;
using TipMolde.Core.Interface.Utilizador.IUser;
using TipMolde.Core.Models;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<User>> SearchByNameAsync(string searchTerm)
        {
            var term = $"%{searchTerm.Trim()}%";
            return await _context.Users
                .AsNoTracking()
                .Where(c => EF.Functions.Like(c.Nome, term))
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }

        public Task<User?> GetByEmailAsync(string email) =>
            _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
    }
}
