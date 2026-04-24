using Microsoft.EntityFrameworkCore;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Utilizador.IUser;
using TipMolde.Domain.Entities;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    /// <summary>
    /// Implementa persistencia de utilizadores no modulo de autenticacao e gestao de acesso.
    /// </summary>
    /// <remarks>
    /// Especializa o repositorio generico com consultas por nome e por email.
    /// </remarks>
    public class UserRepository : GenericRepository<User, int>, IUserRepository
    {
        /// <summary>
        /// Construtor de UserRepository.
        /// </summary>
        /// <param name="context">Contexto de dados da aplicacao.</param>
        public UserRepository(ApplicationDbContext context) : base(context) { }

        /// <summary>
        /// Pesquisa utilizadores por nome.
        /// </summary>
        /// <param name="searchTerm">Termo parcial para pesquisa no nome.</param>
        /// <param name="page">Numero da pagina a ser retornada.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Colecao de utilizadores ordenada alfabeticamente pelo nome.</returns>
        public async Task<PagedResult<User>> SearchByNameAsync(string searchTerm, int page, int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize > 200 ? 200 : pageSize;

            var query = _context.Users
                .AsNoTracking()
                .Where(u => u.Nome.Contains(searchTerm.Trim()));

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(u => u.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<User>(items, totalCount, page, pageSize);
        }

        /// <summary>
        /// Obtem um utilizador pelo email.
        /// </summary>
        /// <param name="email">Email unico do utilizador.</param>
        /// <returns>Utilizador encontrado ou nulo quando nao existe correspondencia.</returns>
        public Task<User?> GetByEmailAsync(string email) =>
            _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
    }
}
