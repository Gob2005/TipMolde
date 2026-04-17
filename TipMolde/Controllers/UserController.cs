using AutoMapper;
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
            // Porque: suportamos emissores JWT diferentes que podem usar "sub" ou NameIdentifier.
            // Risco: alterar esta ordem sem alinhar o emissor pode impedir a resolucao do utilizador autenticado.
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
        private readonly IMapper _mapper;

        public UserController(IUserManagementService userService, ILogger<UserController> logger, IMapper mapper)
        {
            _userService = userService;
            _logger = logger;
            _mapper = mapper;
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest(CreateProblem(
                    StatusCodes.Status400BadRequest,
                    "Pedido invalido",
                    "Os parametros de paginacao sao invalidos. Regras: page >= 1."));
            }

            var result = await _userService.GetAllAsync(page, pageSize);
            var items = _mapper.Map<IEnumerable<ResponseUserDTO>>(result.Items);

            return Ok(new
            {
                result.TotalCount,
                result.CurrentPage,
                result.PageSize,
                Items = items
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

            return Ok(_mapper.Map<ResponseUserDTO>(user));
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
            return Ok(_mapper.Map<IEnumerable<ResponseUserDTO>>(users));
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

            var user = _mapper.Map<User>(dto);

            var createdUser = await _userService.CreateAsync(user);
            _logger.LogInformation("Utilizador {Email} criado com sucesso por admin {AdminId}", dto.Email, GetAuthenticatedUserId());

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = createdUser.User_id },
                _mapper.Map<ResponseUserDTO>(createdUser));
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

            // Porque: apenas ADMIN ou o proprio utilizador podem alterar o perfil alvo.
            // Risco: remover esta regra permite alteracao indevida de contas de terceiros (quebra RBAC).
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

            _mapper.Map(dto, user);

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
    }
}
