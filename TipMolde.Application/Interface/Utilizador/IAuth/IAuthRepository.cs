using TipMolde.Domain.Entities;

namespace TipMolde.Application.Interface.Utilizador.IAuth
{
    /// <summary>
    /// Define operacoes de persistencia para autenticacao de utilizadores.
    /// </summary>
    /// <remarks>
    /// Abstrai o acesso a dados de utilizador necessario ao fluxo de login e migracao de password.
    /// </remarks>
    public interface IAuthRepository
    {
        /// <summary>
        /// Obtem um utilizador pelo endereco de email.
        /// </summary>
        /// <param name="email">Email unico usado na autenticacao.</param>
        /// <returns>Utilizador encontrado ou nulo quando nao existe registo.</returns>
        Task<User?> GetByEmailAsync(string email);
        /// <summary>
        /// Atualiza os dados de um utilizador existente.
        /// </summary>
        /// <param name="user">Entidade de utilizador com alteracoes a persistir.</param>
        /// <returns>Task assincrona concluida apos persistencia.</returns>
        Task UpdateAsync(User user);
    }
}
