using System.ComponentModel.DataAnnotations;

namespace TipMolde.Application.Dtos.UserDto
{
    public class UpdateUserDto
    {
        [MinLength(3), MaxLength(100)]
        public string? Nome { get; set; }

        [EmailAddress, MaxLength(100)]
        public string? Email { get; set; }
    }
}
