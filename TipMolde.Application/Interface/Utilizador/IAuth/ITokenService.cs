using TipMolde.Domain.Entities;

namespace TipMolde.Application.Interface.Utilizador.IAuth
{
    /// <summary>
    /// Define operacoes de emissao de tokens JWT para autenticacao.
    /// </summary>
    /// <remarks>
    /// Centraliza a geracao de credenciais assinadas para acesso a recursos protegidos.
    /// </remarks>
    public interface ITokenService
    {
        /// <summary>
        /// Cria um token de autenticacao para um utilizador.
        /// </summary>
        /// <param name="user">Entidade de utilizador autenticado.</param>
        /// <returns>Token JWT serializado em formato string.</returns>
        string CreateToken(User user);
    }
}
