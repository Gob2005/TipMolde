using TipMolde.Application.Dtos.AuthDto;

namespace TipMolde.Application.Interface.Utilizador.IAuth
{
    /// <summary>
    /// Define casos de uso de autenticacao de utilizadores.
    /// </summary>
    /// <remarks>
    /// Centraliza o contrato funcional para login e encerramento de sessao.
    /// </remarks>
    public interface IAuthService
    {
        /// <summary>
        /// Autentica um utilizador e devolve dados de sessao.
        /// </summary>
        /// <param name="email">Email fornecido para identificacao do utilizador.</param>
        /// <param name="password">Password recebida para validacao de credenciais.</param>
        /// <returns>DTO com token emitido e instante de expiracao.</returns>
        Task<AuthResponseDto> LoginAsync(string email, string password);
        /// <summary>
        /// Termina a sessao associada a um token e regista a revogacao.
        /// </summary>
        /// <param name="token">Token JWT em formato bruto ou cabecalho Bearer.</param>
        /// <returns>Resultado funcional da operacao de logout.</returns>
        Task<LogoutResultDto> LogoutAsync(string token);
    }
}
