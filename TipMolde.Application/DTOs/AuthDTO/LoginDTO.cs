using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.Dtos.AuthDto
{
    public class LoginDto
    {
        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required, MinLength(8), MaxLength(255)]
        public required string Password { get; set; }
    }
}
