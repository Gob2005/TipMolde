using System.ComponentModel.DataAnnotations;

namespace TipMolde.API.DTOs.UserDTO
{
    public class ResetPasswordDTO
    {
        [Required, MinLength(8), MaxLength(255)]
        public required string NewPassword { get; set; }
    }
}