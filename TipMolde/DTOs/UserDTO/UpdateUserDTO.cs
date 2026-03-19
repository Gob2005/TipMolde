using System.ComponentModel.DataAnnotations;

namespace TipMolde.API.DTOs.UserDTO
{
    public class UpdateUserDTO
    {
        [MinLength(5), MaxLength(100)]
        public string? Nome { get; set; }

        [EmailAddress, MaxLength(150)]
        public string? Email { get; set; }
    }
}
