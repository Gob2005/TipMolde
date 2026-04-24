using Microsoft.EntityFrameworkCore;
using System.Threading;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Comercio.ICliente;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Domain.Entities.Producao;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    /// <summary>
    /// Implementa persistencia de clientes no modulo comercial.
    /// </summary>
    /// <remarks>
    /// Especializa o repositorio generico com consultas focadas em pesquisa textual e dados relacionados.
    /// </remarks>
    public class ClienteRepository : GenericRepository<Cliente, int>, IClienteRepository
    {
        /// <summary>
        /// Construtor de ClienteRepository.
        /// </summary>
        /// <param name="context">Contexto de dados da aplicacao.</param>
        public ClienteRepository(ApplicationDbContext context) : base(context) { }

        /// <summary>
        /// Obtem um cliente com as encomendas associadas.
        /// </summary>
        /// <param name="clienteId">Identificador unico do cliente.</param>
        /// <returns>Cliente com encomendas ou nulo quando nao existe registo.</returns>
        public async Task<Cliente?> GetClienteWithEncomendasAsync(int clienteId)
        {
            return await _context.Clientes
                .AsNoTracking()
                .Include(c => c.Encomendas)
                .FirstOrDefaultAsync(c => c.Cliente_id == clienteId);
        }

        /// <summary>
        /// Obtem um cliente pelo NIF.
        /// </summary>
        /// <param name="nif">Numero de identificacao fiscal do cliente.</param>
        /// <returns>Cliente encontrado ou nulo quando nao existe correspondencia.</returns>
        public Task<Cliente?> GetByNifAsync(string nif) =>
            _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.NIF == nif);

        /// <summary>
        /// Obtem um cliente pela sigla.
        /// </summary>
        /// <param name="sigla">Sigla identificadora do cliente.</param>
        /// <returns>Cliente encontrado ou nulo quando nao existe correspondencia.</returns>
        public Task<Cliente?> GetBySiglaAsync(string sigla) =>
            _context.Clientes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Sigla == sigla);

        /// <summary>
        /// Pesquisa clientes por nome.
        /// </summary>
        /// <param name="searchTerm">Termo parcial para pesquisa no nome do cliente.</param>
        /// <param name="page">Numero da pagina a consultar.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Colecao de clientes ordenada alfabeticamente pelo nome.</returns>
        public async Task<PagedResult<Cliente>> SearchByNameAsync(string searchTerm, int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize > 200 ? 200 : pageSize;

            var query = _context.Clientes
                .AsNoTracking()
                .Where(c => c.Nome.Contains(searchTerm));
                

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Cliente>(items, totalCount, page, pageSize);
        }

        /// <summary>
        /// Pesquisa clientes por sigla.
        /// </summary>
        /// <param name="searchTerm">Termo parcial para pesquisa na sigla do cliente.</param>
        /// <param name="page">Numero da pagina a consultar.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Colecao de clientes ordenada alfabeticamente pela sigla.</returns>
        public async Task<PagedResult<Cliente>> SearchBySiglaAsync(string searchTerm, int page = 1, int pageSize = 10)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize > 200 ? 200 : pageSize;

            var query = _context.Clientes
                .AsNoTracking()
                .Where(c => c.Sigla.Contains(searchTerm));


            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.Sigla)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Cliente>(items, totalCount, page, pageSize);
        }
    }
}
