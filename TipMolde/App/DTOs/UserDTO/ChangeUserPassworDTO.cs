namespace TipMolde.App.DTOs.UserDTO
{
    public class ChangeUserPassworDTO
    {
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
