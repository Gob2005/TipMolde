using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.DTOs.UserDTO
{
    public class ResetPasswordDTO
    {
        [Required, MinLength(8), MaxLength(255)]
        public required string NewPassword { get; set; }
    }
}