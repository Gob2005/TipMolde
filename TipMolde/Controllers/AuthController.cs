using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipMolde.API.DTOs.AuthDTO;
using TipMolde.Core.Interface.IAuth;

namespace TipMolde.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var token = await _authService.LoginAsync(dto.Email, dto.Password);
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized(new { message = "Email ou password invalidos." });
            }

            return Ok(new { token });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var authHeader = Request.Headers.Authorization.ToString();
            await _authService.LogoutAsync(authHeader);
            return Ok(new { message = "Sessao terminada com sucesso." });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            await _authService.ChangePasswordAsync(dto.Email, dto.CurrentPassword, dto.NewPassword);
            return Ok();
        }
    }
}
