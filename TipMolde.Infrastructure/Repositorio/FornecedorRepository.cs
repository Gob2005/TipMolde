using Microsoft.EntityFrameworkCore;
using TipMolde.Application.Interface.Comercio.IFornecedor;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class FornecedorRepository : GenericRepository<Fornecedor, int>, IFornecedorRepository
    {
        public FornecedorRepository(ApplicationDbContext context) : base(context) { }

        public Task<Fornecedor?> GetByNifAsync(string nif) =>
            _context.Fornecedores
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.NIF == nif);

        public async Task<IEnumerable<Fornecedor>> SearchByNameAsync(string searchTerm)
        {
            var term = $"%{searchTerm.Trim()}%";
            return await _context.Fornecedores
                .AsNoTracking()
                .Where(f => EF.Functions.Like(f.Nome, term))
                .OrderBy(f => f.Nome)
                .ToListAsync();
        }
    }
}
