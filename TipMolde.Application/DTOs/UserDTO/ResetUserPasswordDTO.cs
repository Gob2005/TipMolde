using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.Dtos.UserDto
{
    public class ResetUserPasswordDto
    {
        [Required, MinLength(8), MaxLength(255)]
        public required string NewPassword { get; set; }
    }
}