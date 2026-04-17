namespace TipMolde.Application.DTOs.AuthDTO
{
    public class AuthResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public DateTimeOffset ExpiresAt { get; set; }
    }
}
