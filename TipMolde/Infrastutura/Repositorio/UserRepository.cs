using Microsoft.EntityFrameworkCore;
using TipMolde.Core.Interface.IUser;
using TipMolde.Core.Models;
using TipMolde.Infrastutura.DB;

namespace TipMolde.Infrastutura.Repositorio
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<User>> SearchByNameAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Enumerable.Empty<User>();
            }

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
