using System.ComponentModel.DataAnnotations;

namespace TipMolde.API.DTOs.UserDTO
{
    public class ChangePasswordDTO
    {
        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required, MinLength(8), MaxLength(255)]
        public required string CurrentPassword { get; set; }

        [Required, MinLength(8), MaxLength(255)]
        public required string NewPassword { get; set; }
    }
}
