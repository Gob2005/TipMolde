using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TipMolde.Application.Dtos.UserDto;
using TipMolde.Application.Interface.Utilizador.IUser;
using TipMolde.Domain.Enums;

namespace TipMolde.API.Controllers
{
    /// <summary>
    /// Fornece utilitarios comuns para controladores que dependem do utilizador autenticado.
    /// </summary>
    /// <remarks>
    /// Centraliza a resolucao do identificador do utilizador a partir das claims do JWT.
    /// </remarks>
    public abstract class AuthenticatedControllerBase : ControllerBase
    {
        /// <summary>
        /// Resolve o identificador do utilizador autenticado a partir das claims do token.
        /// </summary>
        /// <returns>ID do utilizador autenticado.</returns>
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

    /// <summary>
    /// Disponibiliza endpoints para gestao de utilizadores.
    /// </summary>
    /// <remarks>
    /// Implementa operacoes de consulta, criacao, atualizacao, alteracao de perfil e remocao de contas.
    /// </remarks>
    [ApiController]
    [Route("api/users")]
    public class UserController : AuthenticatedControllerBase
    {
        private readonly IUserManagementService _userService;
        private readonly ILogger<UserController> _logger;

        /// <summary>
        /// Construtor de UserController.
        /// </summary>
        /// <param name="userService">Servico responsavel pelos casos de uso de utilizador.</param>
        /// <param name="logger">Logger para rastreabilidade de operacoes administrativas.</param>
        public UserController(IUserManagementService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Lista utilizadores com paginacao.
        /// </summary>
        /// <param name="page">Numero da pagina a consultar.</param>
        /// <param name="pageSize">Quantidade de itens por pagina.</param>
        /// <returns>Resultado HTTP com metadados de paginacao e lista de utilizadores.</returns>
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

            return Ok(new
            {
                result.TotalCount,
                result.CurrentPage,
                result.PageSize,
                Items = result.Items
            });
        }

        /// <summary>
        /// Obtem um utilizador pelo identificador.
        /// </summary>
        /// <param name="id">Identificador unico do utilizador.</param>
        /// <returns>Resultado HTTP com utilizador encontrado ou erro de nao encontrado.</returns>
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

        /// <summary>
        /// Pesquisa utilizadores por nome.
        /// </summary>
        /// <param name="searchTerm">Termo parcial de pesquisa aplicado ao nome do utilizador.</param>
        /// <returns>Resultado HTTP com utilizadores correspondentes ao termo informado.</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpGet("search")]
        public async Task<IActionResult> SearchByName([FromQuery] string searchTerm, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(CreateProblem(
                    StatusCodes.Status400BadRequest,
                    "Pedido invalido",
                    "O parametro searchTerm e obrigatorio."));
            }

            if (page < 1 || pageSize < 1)
            {
                return BadRequest(CreateProblem(
                    StatusCodes.Status400BadRequest,
                    "Pedido invalido",
                    "Os parametros de paginacao sao invalidos. Regras: page >= 1."));
            }

            var users = await _userService.SearchByNameAsync(searchTerm, page, pageSize);
            return Ok(users);
        }

        /// <summary>
        /// Cria um novo utilizador.
        /// </summary>
        /// <remarks>
        /// Valida o body de entrada e regista a operacao com o administrador autenticado.
        /// </remarks>
        /// <param name="dto">Dados de criacao do utilizador.</param>
        /// <returns>Resultado HTTP de criacao com o recurso criado.</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ProducesResponseType(typeof(ResponseUserDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateProblem(
                    StatusCodes.Status400BadRequest,
                    "Pedido invalido",
                    "O body do pedido e invalido."));
            }

            var createdUser = await _userService.CreateAsync(dto);
            _logger.LogInformation("Utilizador {Email} criado com sucesso por admin {AdminId}", dto.Email, GetAuthenticatedUserId());

            return CreatedAtAction(
                nameof(GetUserById),
                new { id = createdUser.User_id },
                createdUser);
        }

        /// <summary>
        /// Atualiza os dados de um utilizador.
        /// </summary>
        /// <remarks>
        /// Permite atualizacao apenas para administradores ou para o proprio utilizador autenticado.
        /// </remarks>
        /// <param name="id">Identificador do utilizador alvo da atualizacao.</param>
        /// <param name="dto">Dados enviados para atualizacao do perfil.</param>
        /// <returns>Resultado HTTP sem conteudo quando a atualizacao e concluida.</returns>
        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
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

            await _userService.UpdateAsync(id, dto);
            return NoContent();
        }

        /// <summary>
        /// Altera o perfil (role) de um utilizador.
        /// </summary>
        /// <param name="id">Identificador do utilizador alvo.</param>
        /// <param name="dto">Dados com o novo perfil a atribuir.</param>
        /// <returns>Resultado HTTP sem conteudo quando a alteracao e concluida.</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id:int}/role")]
        public async Task<IActionResult> ChangeRole(int id, [FromBody] ChangeUserRoleDto dto)
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

        /// <summary>
        /// Remove um utilizador pelo identificador.
        /// </summary>
        /// <param name="id">Identificador unico do utilizador a remover.</param>
        /// <returns>Resultado HTTP sem conteudo quando a remocao e concluida.</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Cria uma resposta de erro padrao no formato ProblemDetails.
        /// </summary>
        /// <param name="status">Codigo de estado HTTP da resposta.</param>
        /// <param name="title">Titulo curto do erro.</param>
        /// <param name="detail">Descricao detalhada do problema.</param>
        /// <returns>Instancia de ProblemDetails com o contexto da falha.</returns>
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
