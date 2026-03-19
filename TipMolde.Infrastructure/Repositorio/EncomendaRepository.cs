using Microsoft.EntityFrameworkCore;
using TipMolde.Core.Enums;
using TipMolde.Core.Interface.IEncomenda;
using TipMolde.Core.Models;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    public class EncomendaRepository : GenericRepository<Encomenda>, IEncomendaRepository
    {
        public EncomendaRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Encomenda>> GetByEstadoAsync(EstadoEncomenda estado)
        {
            return await _context.Encomendas
                .AsNoTracking()
                .Where(e => e.Estado == estado)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<Encomenda?> GetByNumeroEncomendaClienteAsync(string numeroEncomendaCliente)
        {
            return await _context.Encomendas
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.NumeroEncomendaCliente == numeroEncomendaCliente.Trim());
        }

        public async Task<IEnumerable<Encomenda>> GetEncomendasPorConcluirAsync()
        {
            return await _context.Encomendas
                .AsNoTracking()
                .Where(e => e.Estado != EstadoEncomenda.CONCLUIDA
                         && e.Estado != EstadoEncomenda.CANCELADA)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<Encomenda?> GetWithMoldesAsync(int id)
        {
            return await _context.Encomendas
                .AsNoTracking()
                .Include(e => e.Moldes)
                .FirstOrDefaultAsync(e => e.Encomenda_id == id);
        }
    }
}
