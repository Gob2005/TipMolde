using System.ComponentModel.DataAnnotations;

namespace TipMolde.App.DTOs.AuthDTO
{
    public class LoginDTO
    {
        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required, MinLength(8), MaxLength(255)]
        public required string Password { get; set; }
    }
}
