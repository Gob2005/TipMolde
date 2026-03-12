using TipMolde.Core.Enums;

namespace TipMolde.API.DTOs.UserDTO
{
    public class ResponseUserDTO
    {
        public int Id { get; set; }
        public required string Nome { get; set; }
        public required string Email { get; set; }
        public required UserRole Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;


    }
}
