/// <summary>
/// Repositório genérico base com operações CRUD.
/// </summary>
/// <typeparam name="T">Tipo da entidade.</typeparam>
/// <remarks>
/// TKey genérico permite suportar diferentes tipos de ID (int, Guid, string).
/// Paginação implementada através de PagedResult para evitar retornar todos os registos.
/// </remarks>
namespace TipMolde.Application.Interface
{
    public interface IGenericRepository<T, TKey> where T : class
    {
        Task<PagedResult<T>> GetAllAsync(int page, int pageSize);
        Task<T?> GetByIdAsync(TKey id);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(TKey id);
    }

    public record PagedResult<T>(IEnumerable<T> Items, int TotalCount, int CurrentPage, int PageSize);
}

