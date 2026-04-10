using Microsoft.EntityFrameworkCore;
using TipMolde.Application.Interface;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class GenericRepository<T, TKey> : IGenericRepository<T, TKey> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _db;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync() => await _db.AsNoTracking().ToListAsync();

        public Task<PagedResult<T>> GetAllAsync(int page = 1, int pageSize = 50)
        {
            throw new NotImplementedException();
        }

        public async Task<T?> GetByIdAsync(TKey id) => await _db.FindAsync(id);

        public async Task<T> AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _db.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TKey id)
        {
            var entity = await _db.FindAsync(id);
            if (entity is null) return;
            _db.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
