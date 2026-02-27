using Microsoft.AspNetCore.Mvc;
using TipMolde.App.DTOs.UserDTO;
using TipMolde.Core.Interface.IUser;
using TipMolde.Core.Models;



namespace TipMolde.App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> SearchByName(string searchTerm)
        {
            var users = await _userService.SearchByNameAsync(searchTerm);
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome)) return BadRequest("Nome e obrigatorio.");
            if (string.IsNullOrWhiteSpace(dto.Email)) return BadRequest("Email e obrigatorio.");
            if (string.IsNullOrWhiteSpace(dto.Password)) return BadRequest("Password e obrigatoria.");
            if (!Enum.IsDefined(typeof(CreateUserDTO.UserRole), dto.Role)) return BadRequest("Role invalida.");

            var user = new User
            {
                Nome = dto.Nome?.Trim(),
                Email = dto.Email?.Trim(),
                Password = dto.Password,
                Role = (User.UserRole)dto.Role,
                CreatedAt = dto.CreatedAt
            };

            var createdUser = await _userService.CreateUserAsync(user);

            var res = ToResponse(createdUser);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, res);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(User user)
        {
            await _userService.UpdateUserAsync(user);
            var res = ToResponse(user);
            return Ok(res);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null || user.Password != password)
            {
                return Unauthorized();
            }
            // Implement token generation logic here
            return Ok(new { Token = "GeneratedToken" });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Implement logout logic here (e.g., invalidate token)
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null || user.Password != currentPassword)
            {
                return Unauthorized();
            }
            user.Password = newPassword;
            await _userService.UpdateUserAsync(user);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> ChangeRole(ChangeUserRoleDTO dto)
        {
            var user = await _userService.GetUserByIdAsync(dto.Id);
            if (user == null)
            {
                return NotFound();
            }
            user.Role = (User.UserRole)dto.Role;
            await _userService.UpdateUserAsync(user);
            return Ok();
        }

        private static ResponseUserDTO ToResponse(User u) => new()
        {
            Id = u.Id,
            Nome = u.Nome,
            Email = u.Email,
            Password = u.Password,
            Role = (ResponseUserDTO.UserRole)u.Role,
            CreatedAt = u.CreatedAt
        };
    }
}
