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
                .OrderByDescending(e => e.DataRegisto)
                .ToListAsync();
        }

        public async Task<Encomenda?> GetByNumeroEncomendaClienteAsync(string numero)
        {
            return await _context.Encomendas
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.NumeroEncomendaCliente == numero.Trim());
        }

        public async Task<IEnumerable<Encomenda>> GetEncomendasPorConcluirAsync()
        {
            return await _context.Encomendas
                .AsNoTracking()
                .Where(e => e.Estado != EstadoEncomenda.CONCLUIDA
                         && e.Estado != EstadoEncomenda.CANCELADA)
                .OrderByDescending(e => e.DataRegisto)
                .ToListAsync();
        }

        public Task<Encomenda?> GetWithMoldesAsync(int id) =>
            _context.Encomendas
                .Include(e => e.EncomendasMoldes)
                    .ThenInclude(em => em.Molde)
                .FirstOrDefaultAsync(e => e.Encomenda_id == id);
    }
}
