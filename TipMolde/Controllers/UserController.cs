using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TipMolde.Application.DTOs.UserDTO;
using TipMolde.Application.Interface.Utilizador.IUser;
using TipMolde.Domain.Entities;

namespace TipMolde.API.Controllers
{

    /// <summary>
    /// Controller base com helpers comuns para autenticação.
    /// </summary>
    public abstract class AuthenticatedControllerBase : ControllerBase
    {
        /// <summary>
        /// Obtém ID do utilizador autenticado a partir do token JWT.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException">Token inválido ou sem claim de ID.</exception>
        protected int GetAuthenticatedUserId()
        {
            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(sub, out var userId))
                throw new UnauthorizedAccessException("Token invalido ou utilizador nao identificado.");

            return userId;
        }
    }

    /// <summary>
    /// Controller de gestão de utilizadores.
    /// </summary>
    /// <remarks>
    /// Autorização: apenas ADMIN pode criar/editar/eliminar utilizadores.
    /// Utilizadores autenticados podem alterar a própria password.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : AuthenticatedControllerBase
    {
        private readonly IUserManagementService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserManagementService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllAsync();
            return Ok(new
            {
                result.TotalCount,
                result.CurrentPage,
                result.PageSize,
                Items = result.Items
            });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("user-byID")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("search-name")]
        public async Task<IActionResult> SearchByName([FromQuery] string searchTerm)
        {
            var users = await _userService.SearchByNameAsync(searchTerm);
            return Ok(users);
        }

        /// <summary>
        /// Cria novo utilizador (apenas ADMIN).
        /// </summary>
        [Authorize(Roles = "ADMIN")]
        [HttpPost("create-user")]
        [ProducesResponseType(typeof(ResponseUserDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validação de negócio no Service, não no Controller
            var user = new User
            {
                Nome = dto.Nome.Trim(),
                Email = dto.Email.Trim(),
                Password = dto.Password,
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                var createdUser = await _userService.CreateAsync(user);

                _logger.LogInformation("Utilizador {Email} criado com sucesso por admin {AdminId}",
                    dto.Email, GetAuthenticatedUserId());

                return CreatedAtAction(
                    nameof(GetUserById),
                    new { id = createdUser.User_id },
                    ToResponseDto(createdUser));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Falha ao criar utilizador {Email}: {Erro}", dto.Email, ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("update-user/{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            user.Nome = dto.Nome?.Trim() ?? user.Nome;
            user.Email = dto.Email?.Trim() ?? user.Email;

            await _userService.UpdateAsync(user);
            return NoContent();
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("change-role/{id:int}")]
        public async Task<IActionResult> ChangeRole(int id, [FromBody] ChangeUserRoleDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            await _userService.ChangeRoleAsync(id, dto.Role);
            return Ok(user);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }

        // Método helper para conversão
        private static ResponseUserDTO ToResponseDto(User user) => new()
        {
            User_id = user.User_id,
            Nome = user.Nome,
            Email = user.Email,
            Role = user.Role
        };
    }
}
