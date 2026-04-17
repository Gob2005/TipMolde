using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipMolde.Application.DTOs.UserDTO;
using TipMolde.Application.Interface.Utilizador.IUser;

namespace TipMolde.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserPasswordController : AuthenticatedControllerBase
    {
        private readonly IPasswordService _passwordService;
        private readonly ILogger<UserPasswordController> _logger;

        public UserPasswordController(IPasswordService passwordService, ILogger<UserPasswordController> logger)
        {
            _passwordService = passwordService;
            _logger = logger;
        }

        [Authorize]
        [HttpPut("me/password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangeUserPasswordDTO dto)
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

        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id:int}/password/reset")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetUserPasswordDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _passwordService.ResetPasswordAsync(id, dto.NewPassword);
            return NoContent();
        }
    }
}
