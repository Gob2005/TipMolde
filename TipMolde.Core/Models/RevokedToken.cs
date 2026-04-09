namespace TipMolde.Core.Models
{
    /// <summary>
    /// Armazena JTI (JWT Token ID) de tokens revogados para invalidação de sessões.
    /// Permite implementar logout seguro sem necessidade de estado na API.
    /// </summary>
    /// <remarks>
    /// Tokens expirados podem ser removidos periodicamente através de um job de limpeza
    /// (compara ExpiresAtUtc com DateTime.UtcNow).
    /// </remarks>
    public class RevokedToken
    {
        public int RevokedToken_id { get; set; }

        /// <summary>
        /// JWT Token ID único extraído do claim 'jti'.
        /// Indexado para pesquisas rápidas durante validação de tokens.
        /// </summary>
        public required string Jti { get; set; }

        /// <summary>
        /// Data de expiração original do token.
        /// Após esta data, o registo pode ser eliminado da base de dados.
        /// </summary>
        public DateTime ExpiresAtUtc { get; set; }

        /// <summary>
        /// Data em que o token foi revogado (logout ou reset de password).
        /// Útil para auditoria de segurança.
        /// </summary>
        public DateTime RevokedAtUtc { get; set; } = DateTime.UtcNow;
    }
}