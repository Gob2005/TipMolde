using Microsoft.EntityFrameworkCore;
using TipMolde.Application.Interface.Utilizador.IAuth;
using TipMolde.Domain.Entities;
using TipMolde.Infrastructure.DB;

namespace TipMolde.Infrastructure.Repositorio
{
    /// <summary>
    /// Implementa persistencia de dados de autenticacao de utilizadores.
    /// </summary>
    /// <remarks>
    /// Encapsula operacoes de leitura e atualizacao de utilizador usadas no fluxo de login.
    /// </remarks>
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor de AuthRepository.
        /// </summary>
        /// <param name="context">Contexto de dados da aplicacao.</param>
        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtem o utilizador associado ao email indicado.
        /// </summary>
        /// <param name="email">Email usado para pesquisa de utilizador.</param>
        /// <returns>Utilizador encontrado ou nulo quando nao existe correspondencia.</returns>
        public Task<User?> GetByEmailAsync(string email)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Persiste alteracoes de um utilizador existente.
        /// </summary>
        /// <param name="user">Entidade de utilizador com estado atualizado.</param>
        /// <returns>Task assincrona concluida apos gravacao em base de dados.</returns>
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
