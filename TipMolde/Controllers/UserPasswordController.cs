using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipMolde.Application.Dtos.UserDto;
using TipMolde.Application.Interface.Utilizador.IUser;

namespace TipMolde.API.Controllers
{
    /// <summary>
    /// Disponibiliza endpoints para operacoes de password de utilizador.
    /// </summary>
    /// <remarks>
    /// Separa alteracoes de credenciais das restantes operacoes de gestao de utilizadores.
    /// </remarks>
    [ApiController]
    [Route("api/users")]
    public class UserPasswordController : AuthenticatedControllerBase
    {
        private readonly IPasswordService _passwordService;
        private readonly ILogger<UserPasswordController> _logger;

        /// <summary>
        /// Construtor de UserPasswordController.
        /// </summary>
        /// <param name="passwordService">Servico responsavel pela alteracao e reposicao de passwords.</param>
        /// <param name="logger">Logger para auditoria das operacoes de credenciais.</param>
        public UserPasswordController(IPasswordService passwordService, ILogger<UserPasswordController> logger)
        {
            _passwordService = passwordService;
            _logger = logger;
        }

        /// <summary>
        /// Permite ao utilizador autenticado alterar a propria password.
        /// </summary>
        /// <param name="dto">Dados com password atual e nova password pretendida.</param>
        /// <returns>Resultado HTTP da operacao de alteracao de password.</returns>
        [Authorize]
        [HttpPut("me/password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangeUserPasswordDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var userId = GetAuthenticatedUserId();
                await _passwordService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);
                _logger.LogInformation("Utilizador {UserId} alterou password com sucesso", userId);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Permite a um administrador repor a password de um utilizador.
        /// </summary>
        /// <param name="id">Identificador do utilizador alvo da reposicao.</param>
        /// <param name="dto">Dados com a nova password a aplicar.</param>
        /// <returns>Resultado HTTP sem conteudo quando a reposicao e concluida.</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id:int}/password/reset")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetUserPasswordDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _passwordService.ResetPasswordAsync(id, dto.NewPassword);
            return NoContent();
        }
    }
}
