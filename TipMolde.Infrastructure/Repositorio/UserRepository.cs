using Microsoft.EntityFrameworkCore;
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
        /// <returns>Colecao de utilizadores ordenada alfabeticamente pelo nome.</returns>
        public async Task<IEnumerable<User>> SearchByNameAsync(string searchTerm)
        {
            var term = searchTerm.Trim();
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Nome.Contains(term))
                .OrderBy(u => u.Nome)
                .ToListAsync();
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
