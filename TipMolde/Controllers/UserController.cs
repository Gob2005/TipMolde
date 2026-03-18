using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipMolde.API.DTOs.UserDTO;
using TipMolde.Core.Interface.IUser;
using TipMolde.Core.Models;

namespace TipMolde.API.Controllers
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

        [Authorize(Roles = "ADMIN")]
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nome)) return BadRequest("Nome e obrigatorio.");
            if (string.IsNullOrWhiteSpace(dto.Email)) return BadRequest("Email e obrigatorio.");
            if (string.IsNullOrWhiteSpace(dto.Password)) return BadRequest("Password e obrigatoria.");
            if (!Enum.IsDefined(dto.Role)) return BadRequest("Role invalida.");

            var user = new User
            {
                Nome = dto.Nome.Trim(),
                Email = dto.Email.Trim(),
                Password = dto.Password,
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userService.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.User_id }, ToResponse(createdUser));
        }

        [Authorize]
        [HttpPut("update-user/{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO dto)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            user.Nome = dto.Nome?.Trim() ?? user.Nome;
            user.Email = dto.Email?.Trim() ?? user.Email;
            user.Password = dto.Password ?? user.Password;

            await _userService.UpdateUserAsync(user);
            return NoContent();
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("change-role/{id:int}")]
        public async Task<IActionResult> ChangeRole(int id, [FromBody] ChangeUserRoleDTO dto)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            await _userService.ChangeRoleAsync(id, dto.Role);
            return Ok(ToResponse(user));
        }

        private static ResponseUserDTO ToResponse(User u) => new()
        {
            User_id = u.User_id,
            Nome = u.Nome,
            Email = u.Email,
            Role = u.Role,
            CreatedAt = u.CreatedAt
        };
    }
}
