namespace TipMolde.Application.Dtos.AuthDto
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTimeOffset ExpiresAt { get; set; }
    }
}
