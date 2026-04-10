using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TipMolde.Application.DTOs.UserDTO;
using TipMolde.Application.Interface.Utilizador.IUser;

namespace TipMolde.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserPasswordController : ControllerBase
    {
        private readonly IPasswordService _passwordService;
        private readonly ILogger<UserPasswordController> _logger;


        public UserPasswordController(IPasswordService passwordService, ILogger<UserPasswordController> logger)
        {
            _passwordService = passwordService;
            _logger = logger;
        }

        /// <summary>
        /// Altera password do utilizador autenticado.
        /// </summary>
        [Authorize]
        [HttpPut("change-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!int.TryParse(sub, out var userId))
                    throw new UnauthorizedAccessException("Token invalido ou utilizador nao identificado.");

                await _passwordService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);

                _logger.LogInformation("Utilizador {UserId} alterou password com sucesso", userId);

                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Falha ao alterar password: {Erro}", ex.Message);
                return Unauthorized(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("reset-password/{id:int}")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _passwordService.ResetPasswordAsync(id, dto.NewPassword);
            return NoContent();
        }
    }
}
