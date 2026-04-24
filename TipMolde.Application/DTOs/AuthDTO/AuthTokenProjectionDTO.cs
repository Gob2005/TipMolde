namespace TipMolde.Application.Dtos.AuthDto
{
    public class AuthTokenProjectionDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTimeOffset ExpiresAt { get; set; }
    }
}
