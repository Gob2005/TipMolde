using System.ComponentModel.DataAnnotations;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.Dtos.UserDto
{
    public class CreateUserDto
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
