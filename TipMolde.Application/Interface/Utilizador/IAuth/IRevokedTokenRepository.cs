namespace TipMolde.Application.Interface.Utilizador.IAuth
{
    /// <summary>
    /// Define operacoes de armazenamento para tokens JWT revogados.
    /// </summary>
    /// <remarks>
    /// Suporta a invalidacao explicita de sessoes em fluxos de logout.
    /// </remarks>
    public interface IRevokedTokenRepository
    {
        /// <summary>
        /// Verifica se um identificador de token ja foi revogado.
        /// </summary>
        /// <param name="jti">Identificador unico do token JWT.</param>
        /// <returns>Verdadeiro quando o token esta marcado como revogado.</returns>
        Task<bool> IsRevokedAsync(string jti);

        /// <summary>
        /// Regista um token como revogado ate ao seu limite de expiracao.
        /// </summary>
        /// <param name="jti">Identificador unico do token JWT.</param>
        /// <param name="expiresAtUtc">Data UTC em que o token deixa de ser valido.</param>
        /// <returns>Task assincrona concluida apos persistencia da revogacao.</returns>
        Task RevokeAsync(string jti, DateTime expiresAtUtc);
    }
}
