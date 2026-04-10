using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TipMolde.Application.DTOs.AuthDTO;
using TipMolde.Application.Interface.Utilizador.IAuth;

/// <summary>
/// Controller de autenticação.
/// </summary>
/// <remarks>
/// Responsável exclusivamente por login/logout.
/// Gestão de utilizadores está em UserController.
/// </remarks>
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

    /// <summary>
    /// Autentica utilizador e retorna token JWT.
    /// </summary>
    /// <response code="200">Autenticação bem-sucedida.</response>
    /// <response code="401">Credenciais inválidas.</response>
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

            _logger.LogInformation("Login bem-sucedido para utilizador com email {Email}",
                dto.Email);

            return Ok("Token criado");
        }
        catch (UnauthorizedAccessException)
        {
            _logger.LogWarning("Tentativa de login falhada para email {Email}",
                dto.Email);

            return Unauthorized(new { message = "Credenciais invalidas." });
        }
    }

    /// <summary>
    /// Termina sessão do utilizador revogando o token.
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        var authHeader = Request.Headers.Authorization.ToString();
        await _authService.LogoutAsync(authHeader);

        _logger.LogInformation("Utilizador terminou sessão");

        return Ok(new { message = "Sessao terminada com sucesso." });
    }
}