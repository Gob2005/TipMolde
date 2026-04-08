namespace TipMolde.Core.Models
{
    public class RevokedToken
    {
        public int RevokedToken_id { get; set; }
        public required string Jti { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime RevokedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
