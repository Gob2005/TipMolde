using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TipMolde.API.Controllers;
using TipMolde.Application.DTOs.UserDTO;
using TipMolde.Application.Interface.Utilizador.IUser;

namespace TipMolde.Tests.Unitario.Controller;

[TestFixture]
[Category("Unit")]
public class UserPasswordControllerTests
{
    private Mock<IPasswordService> _passwordService = null!;
    private Mock<ILogger<UserPasswordController>> _logger = null!;
    private UserPasswordController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _passwordService = new Mock<IPasswordService>();
        _logger = new Mock<ILogger<UserPasswordController>>();
        _controller = new UserPasswordController(_passwordService.Object, _logger.Object);
    }

    private static void SetAuthenticatedUser(ControllerBase controller, params Claim[] claims)
    {
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"))
            }
        };
    }

    [Test]
    public async Task shouldReturnNoContentWhenChangingPasswordWithValidRequest()
    {
        SetAuthenticatedUser(_controller, new Claim(JwtRegisteredClaimNames.Sub, "10"));
        var dto = new ChangeUserPasswordDTO { CurrentPassword = "Atual123!", NewPassword = "Nova123!" };

        var result = await _controller.ChangePassword(dto);

        result.Should().BeOfType<NoContentResult>();
        _passwordService.Verify(s => s.ChangePasswordAsync(10, dto.CurrentPassword, dto.NewPassword), Times.Once);
    }

    [Test]
    public async Task shouldReturnUnauthorizedWhenUserClaimIsMissing()
    {
        SetAuthenticatedUser(_controller);
        var dto = new ChangeUserPasswordDTO { CurrentPassword = "Atual123!", NewPassword = "Nova123!" };

        var result = await _controller.ChangePassword(dto);

        result.Should().BeOfType<UnauthorizedObjectResult>();
        _passwordService.Verify(s => s.ChangePasswordAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task shouldReturnBadRequestWhenServiceThrowsArgumentException()
    {
        SetAuthenticatedUser(_controller, new Claim(JwtRegisteredClaimNames.Sub, "5"));
        _passwordService
            .Setup(s => s.ChangePasswordAsync(5, It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new ArgumentException("Password fraca"));

        var dto = new ChangeUserPasswordDTO { CurrentPassword = "Atual123!", NewPassword = "fraca" };
        var result = await _controller.ChangePassword(dto);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test]
    public async Task shouldReturnNotFoundWhenServiceThrowsKeyNotFoundException()
    {
        SetAuthenticatedUser(_controller, new Claim(JwtRegisteredClaimNames.Sub, "500"));
        _passwordService
            .Setup(s => s.ChangePasswordAsync(500, It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new KeyNotFoundException("Utilizador nao encontrado"));

        var dto = new ChangeUserPasswordDTO { CurrentPassword = "Atual123!", NewPassword = "Nova123!" };
        var result = await _controller.ChangePassword(dto);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test]
    public async Task shouldReturnNoContentWhenResettingPassword()
    {
        var dto = new ResetUserPasswordDTO { NewPassword = "Admin123!" };

        var result = await _controller.ResetPassword(7, dto);

        result.Should().BeOfType<NoContentResult>();
        _passwordService.Verify(s => s.ResetPasswordAsync(7, "Admin123!"), Times.Once);
    }

    [Test]
    public async Task shouldReturnBadRequestWhenChangingPasswordWithInvalidModelState()
    {
        _controller.ModelState.AddModelError("NewPassword", "Obrigatorio");

        var result = await _controller.ChangePassword(new ChangeUserPasswordDTO { CurrentPassword = null, NewPassword = null});

        result.Should().BeOfType<BadRequestObjectResult>();
        _passwordService.Verify(s => s.ChangePasswordAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task shouldReturnUnauthorizedWhenServiceThrowsUnauthorizedAccessException()
    {
        SetAuthenticatedUser(_controller, new Claim(JwtRegisteredClaimNames.Sub, "5"));
        _passwordService
            .Setup(s => s.ChangePasswordAsync(5, It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new UnauthorizedAccessException("Password atual invalida"));

        var result = await _controller.ChangePassword(new ChangeUserPasswordDTO
        {
            CurrentPassword = "Atual123!",
            NewPassword = "Nova123!"
        });

        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Test]
    public async Task shouldReturnBadRequestWhenResettingPasswordWithInvalidModelState()
    {
        _controller.ModelState.AddModelError("NewPassword", "Obrigatorio");

        var result = await _controller.ResetPassword(8, new ResetUserPasswordDTO { NewPassword = null });

        result.Should().BeOfType<BadRequestObjectResult>();
        _passwordService.Verify(s => s.ResetPasswordAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    }
}
