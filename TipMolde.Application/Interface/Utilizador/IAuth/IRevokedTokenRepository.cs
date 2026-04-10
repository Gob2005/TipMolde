namespace TipMolde.Application.Interface.Utilizador.IAuth
{
    public interface IRevokedTokenRepository
    {
        Task<bool> IsRevokedAsync(string jti);
        Task RevokeAsync(string jti, DateTime expiresAtUtc);
    }
}
