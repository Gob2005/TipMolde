using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TipMolde.API.Controllers;
using TipMolde.Application.DTOs.UserDTO;
using TipMolde.Application.Interface;
using TipMolde.Application.Interface.Utilizador.IUser;
using TipMolde.Domain.Enums;

namespace TipMolde.Tests.Unitario.Controller;

[TestFixture]
[Category("Unit")]
public class UserControllerTests
{
    private Mock<IUserManagementService> _userService = null!;
    private Mock<ILogger<UserController>> _logger = null!;
    private UserController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _userService = new Mock<IUserManagementService>();
        _logger = new Mock<ILogger<UserController>>();
        _controller = new UserController(_userService.Object, _logger.Object);
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
    public async Task shouldReturnResponseUserDtoWhenGettingById()
    {
        _userService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new ResponseUserDTO
        {
            User_id = 1,
            Nome = "Ana",
            Email = "ana@tipmolde.pt",
            Role = UserRole.ADMIN
        });

        var result = await _controller.GetUserById(1);

        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeOfType<ResponseUserDTO>();
    }

    [Test]
    public async Task shouldReturnForbiddenWhenNonAdminTriesToUpdateAnotherUser()
    {
        SetAuthenticatedUser(_controller, new Claim(JwtRegisteredClaimNames.Sub, "2"));

        _userService.Setup(s => s.GetByIdAsync(2)).ReturnsAsync(new ResponseUserDTO
        {
            User_id = 2,
            Nome = "User2",
            Email = "u2@tipmolde.pt",
            Role = UserRole.GESTOR_PRODUCAO
        });

        var dto = new UpdateUserDTO { Nome = "Novo Nome", Email = "novo@tipmolde.pt" };
        var result = await _controller.UpdateUser(5, dto);

        var forbidden = result as ObjectResult;
        forbidden.Should().NotBeNull();
        forbidden!.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        _userService.Verify(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<UpdateUserDTO>()), Times.Never);
    }

    [Test]
    public async Task shouldUpdateWhenUserUpdatesOwnData()
    {
        SetAuthenticatedUser(_controller, new Claim(JwtRegisteredClaimNames.Sub, "2"));

        var sameUser = new ResponseUserDTO
        {
            User_id = 2,
            Nome = "Nome Antigo",
            Email = "old@tipmolde.pt",
            Role = UserRole.GESTOR_PRODUCAO
        };

        _userService.Setup(s => s.GetByIdAsync(2)).ReturnsAsync(sameUser);

        var dto = new UpdateUserDTO { Nome = "  Novo Nome  ", Email = null };
        var result = await _controller.UpdateUser(2, dto);

        result.Should().BeOfType<NoContentResult>();
        _userService.Verify(s => s.UpdateAsync(2, It.Is<UpdateUserDTO>(u => u.Nome == "  Novo Nome  " && u.Email == null)), Times.Once);
    }

    [Test]
    public async Task shouldReturnBadRequestWhenPagingParametersAreInvalid()
    {
        var result = await _controller.GetAllUsers(0, 10);
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test]
    public async Task shouldReturnPagedResponseWithDtoItemsWhenGettingAllUsers()
    {
        var users = new List<ResponseUserDTO>
        {
            new()
            {
                User_id = 3,
                Nome = "Bruno",
                Email = "bruno@tipmolde.pt",
                Role = UserRole.ADMIN
            }
        };

        _userService.Setup(s => s.GetAllAsync(1, 10))
            .ReturnsAsync(new PagedResult<ResponseUserDTO>(users, 1, 1, 10));

        var result = await _controller.GetAllUsers(1, 10);

        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();

        var payload = ok!.Value!;
        var itemsProp = payload.GetType().GetProperty("Items");
        itemsProp.Should().NotBeNull();

        var items = itemsProp!.GetValue(payload);
        items.Should().BeAssignableTo<IEnumerable<ResponseUserDTO>>();
    }
}
