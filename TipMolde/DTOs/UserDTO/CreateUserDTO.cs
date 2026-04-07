using System.ComponentModel.DataAnnotations;
using TipMolde.Core.Enums;

namespace TipMolde.API.DTOs.UserDTO
{
    public class CreateUserDTO
    {
        [Required, MinLength(5), MaxLength(100)]
        public required string Nome { get; set; }

        [Required, EmailAddress, MaxLength(150)]
        public required string Email { get; set; }

        [Required, MinLength(8), MaxLength(255)]
        public required string Password { get; set; }

        [Required]
        public UserRole Role { get; set; }
    }
}
