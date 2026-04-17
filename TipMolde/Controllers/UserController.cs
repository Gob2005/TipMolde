using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TipMolde.Application.DTOs.UserDTO;
using TipMolde.Application.Interface.Utilizador.IUser;
using TipMolde.Domain.Entities;
using TipMolde.Domain.Enums;

namespace TipMolde.API.Controllers
{
    public abstract class AuthenticatedControllerBase : ControllerBase
    {
        protected int GetAuthenticatedUserId()
        {
            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(sub, out var userId))
                throw new UnauthorizedAccessException("Token invalido ou utilizador nao identificado.");

            return userId;
        }
    }

    [ApiController]
    [Route("api/users")]
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
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1 )
            {
                return BadRequest(CreateProblem(
                    StatusCodes.Status400BadRequest,
                    "Pedido invalido",
                    $"Os parametros de paginacao sao invalidos. Regras: page >= 1."));
            }

            var result = await _userService.GetAllAsync(page, pageSize);
            return Ok(new
            {
                result.TotalCount,
                result.CurrentPage,
                result.PageSize,
                Items = result.Items
            });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(CreateProblem(
                    StatusCodes.Status404NotFound,
                    "Recurso nao encontrado",
                    $"Utilizador com ID {id} nao encontrado."));
            }

            return Ok(user);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchByName([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(CreateProblem(
                    StatusCodes.Status400BadRequest,
                    "Pedido invalido",
                    "O parametro searchTerm e obrigatorio."));
            }

            var users = await _userService.SearchByNameAsync(searchTerm);
            return Ok(users);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ProducesResponseType(typeof(ResponseUserDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateProblem(
                    StatusCodes.Status400BadRequest,
                    "Pedido invalido",
                    "O body do pedido e invalido."));
            }

            var user = new User
            {
                Nome = dto.Nome.Trim(),
                Email = dto.Email.Trim(),
                Password = dto.Password,
                Role = dto.Role,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userService.CreateAsync(user);
            _logger.LogInformation("Utilizador {Email} criado com sucesso por admin {AdminId}", dto.Email, GetAuthenticatedUserId());

            return CreatedAtAction(nameof(GetUserById), new { id = createdUser.User_id }, ToResponseDto(createdUser));
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateProblem(
                    StatusCodes.Status400BadRequest,
                    "Pedido invalido",
                    "O body do pedido e invalido."));
            }

            int authenticatedUserId;
            try
            {
                authenticatedUserId = GetAuthenticatedUserId();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(CreateProblem(
                    StatusCodes.Status401Unauthorized,
                    "Nao autorizado",
                    ex.Message));
            }

            var authenticatedUser = await _userService.GetByIdAsync(authenticatedUserId);
            if (authenticatedUser == null)
            {
                return Unauthorized(CreateProblem(
                    StatusCodes.Status401Unauthorized,
                    "Nao autorizado",
                    "Utilizador autenticado nao encontrado."));
            }

            if (authenticatedUser.Role != UserRole.ADMIN && authenticatedUserId != id)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    CreateProblem(StatusCodes.Status403Forbidden, "Proibido", "Sem permissao para atualizar este utilizador."));
            }

            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(CreateProblem(
                    StatusCodes.Status404NotFound,
                    "Recurso nao encontrado",
                    $"Utilizador com ID {id} nao encontrado."));
            }

            user.Nome = dto.Nome?.Trim() ?? user.Nome;
            user.Email = dto.Email?.Trim() ?? user.Email;

            await _userService.UpdateAsync(user);
            return NoContent();
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id:int}/role")]
        public async Task<IActionResult> ChangeRole(int id, [FromBody] ChangeUserRoleDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateProblem(
                    StatusCodes.Status400BadRequest,
                    "Pedido invalido",
                    "O body do pedido e invalido."));
            }

            await _userService.ChangeRoleAsync(id, dto.Role);
            return NoContent();
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }

        private ProblemDetails CreateProblem(int status, string title, string detail)
        {
            return new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail,
                Instance = HttpContext?.Request?.Path
            };
        }

        private static ResponseUserDTO ToResponseDto(User user) => new()
        {
            User_id = user.User_id,
            Nome = user.Nome,
            Email = user.Email,
            Role = user.Role
        };
    }
}
