using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipMolde.Application.DTOs.AuthDTO;
using TipMolde.Application.Interface.Utilizador.IAuth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await _authService.LoginAsync(dto.Email, dto.Password);

            _logger.LogInformation("Login bem-sucedido para utilizador com email {Email}", dto.Email);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            _logger.LogWarning("Tentativa de login falhada para email {Email}", dto.Email);
            return Unauthorized(new { message = "Credenciais invalidas." });
        }
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Logout()
    {
        var authHeader = Request.Headers.Authorization.ToString();
        var result = await _authService.LogoutAsync(authHeader);

        if (!result.Success)
            return BadRequest(new { message = result.Message });

        _logger.LogInformation("Utilizador terminou sessao");
        return Ok(new { message = result.Message });
    }
}
