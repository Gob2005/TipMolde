using TipMolde.Core.Enums;

namespace TipMolde.API.DTOs.UserDTO
{
    public class ResponseUserDTO
    {
        public int User_id { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public UserRole Role { get; set; }
    }
}
