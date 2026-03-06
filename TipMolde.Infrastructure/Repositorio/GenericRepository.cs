using Microsoft.EntityFrameworkCore;
using TipMolde.Core.Interface;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _db;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _db.AsNoTracking().ToListAsync();

        public async Task<T?> GetByIdAsync(int id) => await _db.FindAsync(id);

        public async Task AddAsync(T entity)
        {
            await _db.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _db.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.FindAsync(id);
            if (entity is null) return;
            _db.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
