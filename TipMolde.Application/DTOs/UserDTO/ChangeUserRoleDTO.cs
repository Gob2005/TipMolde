using System.ComponentModel.DataAnnotations;
using TipMolde.Domain.Enums;

namespace TipMolde.Application.Dtos.UserDto
{
    public class ChangeUserRoleDto
    {
        [Required]
        public required UserRole Role { get; set; }
    }
}
