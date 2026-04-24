using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.Dtos.UserDto
{
    public class ChangeUserPasswordDto
    {
        [Required, MinLength(8), MaxLength(255)]
        public required string CurrentPassword { get; set; }

        [Required, MinLength(8), MaxLength(255)]
        public required string NewPassword { get; set; }
    }
}
