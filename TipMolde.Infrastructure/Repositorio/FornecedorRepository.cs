using Microsoft.EntityFrameworkCore;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Comercio.IFornecedor;
using TipMolde.Domain.Entities.Comercio;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    /// <summary>
    /// Implementa persistencia de fornecedores no modulo comercial.
    /// </summary>
    /// <remarks>
    /// Especializa o repositorio generico com consultas focadas em pesquisa textual e validacao de unicidade por NIF.
    /// </remarks>
    public class FornecedorRepository : GenericRepository<Fornecedor, int>, IFornecedorRepository
    {
        /// <summary>
        /// Construtor de FornecedorRepository.
        /// </summary>
        /// <param name="context">Contexto de dados da aplicacao.</param>
        public FornecedorRepository(ApplicationDbContext context) : base(context) { }

        /// <summary>
        /// Obtem um fornecedor pelo NIF.
        /// </summary>
        /// <param name="nif">Numero de identificacao fiscal do fornecedor.</param>
        /// <returns>Fornecedor encontrado ou nulo quando nao existe correspondencia.</returns>
        public Task<Fornecedor?> GetByNifAsync(string nif) =>
            _context.Fornecedores
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.NIF == nif);

        /// <summary>
        /// Pesquisa fornecedores por nome.
        /// </summary>
        /// <remarks>
        /// O contrato desta pesquisa e parcial, por isso o consumidor nao precisa de enviar wildcards SQL.
        /// </remarks>
        /// <param name="searchTerm">Termo parcial para pesquisa no nome do fornecedor.</param>
        /// <param name="page">Numero da pagina a consultar.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Colecao de fornecedores ordenada alfabeticamente pelo nome.</returns>
        public async Task<PagedResult<Fornecedor>> SearchByNameAsync(string searchTerm, int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize > 200 ? 200 : pageSize;

            var normalizedSearchTerm = searchTerm.Trim();

            var query = _context.Fornecedores
                .AsNoTracking()
                .Where(f => f.Nome.Contains(normalizedSearchTerm));

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(f => f.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Fornecedor>(items, totalCount, page, pageSize);
        }
    }
}