using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users.Select(ToResponse));
        }

        [HttpGet("user-byID")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(ToResponse(user));
        }

        [HttpGet("search-name")]
        public async Task<IActionResult> SearchByName([FromQuery] string searchTerm)
        {
            var users = await _userService.SearchByNameAsync(searchTerm);
            return Ok(users.Select(ToResponse));
        }

        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome)) return BadRequest("Nome e obrigatorio.");
            if (string.IsNullOrWhiteSpace(dto.Email)) return BadRequest("Email e obrigatorio.");
            if (string.IsNullOrWhiteSpace(dto.Password)) return BadRequest("Password e obrigatoria.");
            if (!Enum.IsDefined(typeof(CreateUserDTO.UserRole), dto.Role)) return BadRequest("Role invalida.");

            var user = new User
            {
                Nome = dto.Nome.Trim(),
                Email = dto.Email.Trim(),
                Password = dto.Password,
                Role = (User.UserRole)dto.Role,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, ToResponse(createdUser));
        }

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDTO dto)
        {
            var user = await _userService.GetUserByIdAsync(dto.Id);
            if (user == null) return NotFound();
            await _userService.UpdateUserAsync(user);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("change-role")]
        public async Task<IActionResult> ChangeRole([FromBody] ChangeUserRoleDTO dto)
        {
            var user = await _userService.GetUserByIdAsync(dto.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.Role = (User.UserRole)dto.Role;
            await _userService.UpdateUserAsync(user);
            return Ok(ToResponse(user));
        }

        private static ResponseUserDTO ToResponse(User u) => new()
        {
            Id = u.Id,
            Nome = u.Nome,
            Email = u.Email,
            Role = (ResponseUserDTO.UserRole)u.Role,
            CreatedAt = u.CreatedAt
        };
    }
}
