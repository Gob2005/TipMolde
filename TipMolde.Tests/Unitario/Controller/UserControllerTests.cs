using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TipMolde.API.Controllers;
using TipMolde.Application.Dtos.UserDto;
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
        _userService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(new ResponseUserDto
        {
            User_id = 1,
            Nome = "Ana",
            Email = "ana@tipmolde.pt",
            Role = UserRole.ADMIN
        });

        var result = await _controller.GetUserById(1);

        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeOfType<ResponseUserDto>();
    }

    [Test]
    public async Task shouldReturnForbiddenWhenNonAdminTriesToUpdateAnotherUser()
    {
        SetAuthenticatedUser(_controller, new Claim(JwtRegisteredClaimNames.Sub, "2"));

        _userService.Setup(s => s.GetByIdAsync(2)).ReturnsAsync(new ResponseUserDto
        {
            User_id = 2,
            Nome = "User2",
            Email = "u2@tipmolde.pt",
            Role = UserRole.GESTOR_PRODUCAO
        });

        var dto = new UpdateUserDto { Nome = "Novo Nome", Email = "novo@tipmolde.pt" };
        var result = await _controller.UpdateUser(5, dto);

        var forbidden = result as ObjectResult;
        forbidden.Should().NotBeNull();
        forbidden!.StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        _userService.Verify(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<UpdateUserDto>()), Times.Never);
    }

    [Test]
    public async Task shouldUpdateWhenUserUpdatesOwnData()
    {
        SetAuthenticatedUser(_controller, new Claim(JwtRegisteredClaimNames.Sub, "2"));

        var sameUser = new ResponseUserDto
        {
            User_id = 2,
            Nome = "Nome Antigo",
            Email = "old@tipmolde.pt",
            Role = UserRole.GESTOR_PRODUCAO
        };

        _userService.Setup(s => s.GetByIdAsync(2)).ReturnsAsync(sameUser);

        var dto = new UpdateUserDto { Nome = "  Novo Nome  ", Email = null };
        var result = await _controller.UpdateUser(2, dto);

        result.Should().BeOfType<NoContentResult>();
        _userService.Verify(s => s.UpdateAsync(2, It.Is<UpdateUserDto>(u => u.Nome == "  Novo Nome  " && u.Email == null)), Times.Once);
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
        var users = new List<ResponseUserDto>
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
            .ReturnsAsync(new PagedResult<ResponseUserDto>(users, 1, 1, 10));

        var result = await _controller.GetAllUsers(1, 10);

        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();

        var payload = ok!.Value!;
        var itemsProp = payload.GetType().GetProperty("Items");
        itemsProp.Should().NotBeNull();

        var items = itemsProp!.GetValue(payload);
        items.Should().BeAssignableTo<IEnumerable<ResponseUserDto>>();
    }

    [Test]
    public async Task shouldReturnNotFoundWhenGettingByIdAndUserDoesNotExist()
    {
        _userService.Setup(s => s.GetByIdAsync(44)).ReturnsAsync((ResponseUserDto?)null);

        var result = await _controller.GetUserById(44);

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test]
    public async Task shouldReturnBadRequestWhenSearchingByNameWithBlankTerm()
    {
        var result = await _controller.SearchByName("   ");

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test]
    public async Task shouldReturnBadRequestWhenSearchingByNameWithInvalidPaging()
    {
        var result = await _controller.SearchByName("Ana", 0, 10);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Test]
    public async Task shouldReturnOkWhenSearchingByNameWithValidRequest()
    {
        var users = new List<ResponseUserDto> { BuildUserDto(id: 9, nome: "Ana") };
        _userService.Setup(s => s.SearchByNameAsync("Ana", 2, 5))
            .ReturnsAsync(new PagedResult<ResponseUserDto>(users, 1, 2, 5));

        var result = await _controller.SearchByName("Ana", 2, 5);

        var ok = result as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.Value.Should().BeEquivalentTo(new PagedResult<ResponseUserDto>(users, 1, 2, 5));
    }

    [Test]
    public async Task shouldReturnBadRequestWhenCreatingUserWithInvalidModelState()
    {
        SetAuthenticatedUser(_controller, new Claim(JwtRegisteredClaimNames.Sub, "1"));
        _controller.ModelState.AddModelError("Email", "Obrigatorio");

        var result = await _controller.CreateUser(new CreateUserDto { Email = null , Nome = null, Password = null, Role = UserRole.ADMIN });

        result.Should().BeOfType<BadRequestObjectResult>();
        _userService.Verify(s => s.CreateAsync(It.IsAny<CreateUserDto>()), Times.Never);
    }

    [Test]
    public async Task shouldReturnCreatedAtActionWhenCreatingUserWithValidRequest()
    {
        SetAuthenticatedUser(_controller, new Claim(JwtRegisteredClaimNames.Sub, "1"));

        var dto = new CreateUserDto
        {
            Nome = "Ana",
            Email = "ana@tipmolde.pt",
            Password = "Valida123!",
            Role = UserRole.ADMIN
        };

        var createdUser = BuildUserDto(id: 12, nome: "Ana", email: "ana@tipmolde.pt", role: UserRole.ADMIN);
        _userService.Setup(s => s.CreateAsync(dto)).ReturnsAsync(createdUser);

        var result = await _controller.CreateUser(dto);

        var created = result as CreatedAtActionResult;
        created.Should().NotBeNull();
        created!.ActionName.Should().Be(nameof(UserController.GetUserById));
        created.RouteValues!["id"].Should().Be(12);
        created.Value.Should().BeEquivalentTo(createdUser);
    }

    [Test]
    public async Task shouldReturnBadRequestWhenUpdatingUserWithInvalidModelState()
    {
        _controller.ModelState.AddModelError("Nome", "Obrigatorio");

        var result = await _controller.UpdateUser(1, new UpdateUserDto { Nome = "Ana" });

        result.Should().BeOfType<BadRequestObjectResult>();
        _userService.Verify(s => s.UpdateAsync(It.IsAny<int>(), It.IsAny<UpdateUserDto>()), Times.Never);
    }

    [Test]
    public async Task shouldReturnUnauthorizedWhenUpdatingUserWithInvalidToken()
    {
        SetAuthenticatedUser(_controller);

        var result = await _controller.UpdateUser(1, new UpdateUserDto { Nome = "Ana" });

        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Test]
    public async Task shouldReturnUnauthorizedWhenAuthenticatedUserDoesNotExist()
    {
        SetAuthenticatedUser(_controller, new Claim(JwtRegisteredClaimNames.Sub, "2"));
        _userService.Setup(s => s.GetByIdAsync(2)).ReturnsAsync((ResponseUserDto?)null);

        var result = await _controller.UpdateUser(2, new UpdateUserDto { Nome = "Ana" });

        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Test]
    public async Task shouldReturnNotFoundWhenTargetUserDoesNotExistDuringUpdate()
    {
        SetAuthenticatedUser(_controller, new Claim(JwtRegisteredClaimNames.Sub, "1"));

        _userService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(BuildUserDto(id: 1, role: UserRole.ADMIN));
        _userService.Setup(s => s.GetByIdAsync(5)).ReturnsAsync((ResponseUserDto?)null);

        var result = await _controller.UpdateUser(5, new UpdateUserDto { Nome = "Novo Nome" });

        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Test]
    public async Task shouldReturnBadRequestWhenChangingRoleWithInvalidModelState()
    {
        _controller.ModelState.AddModelError("Role", "Obrigatorio");

        var result = await _controller.ChangeRole(4, new ChangeUserRoleDto { Role = UserRole.ADMIN });

        result.Should().BeOfType<BadRequestObjectResult>();
        _userService.Verify(s => s.ChangeRoleAsync(It.IsAny<int>(), It.IsAny<UserRole>()), Times.Never);
    }

    [Test]
    public async Task shouldReturnNoContentWhenChangingRoleWithValidRequest()
    {
        var result = await _controller.ChangeRole(4, new ChangeUserRoleDto { Role = UserRole.ADMIN });

        result.Should().BeOfType<NoContentResult>();
        _userService.Verify(s => s.ChangeRoleAsync(4, UserRole.ADMIN), Times.Once);
    }

    [Test]
    public async Task shouldReturnNoContentWhenDeletingUserWithValidRequest()
    {
        var result = await _controller.DeleteUser(6);

        result.Should().BeOfType<NoContentResult>();
        _userService.Verify(s => s.DeleteAsync(6), Times.Once);
    }

    private static ResponseUserDto BuildUserDto(
        int id = 1,
        string nome = "Ana",
        string email = "ana@tipmolde.pt",
        UserRole role = UserRole.ADMIN)
    {
        return new ResponseUserDto
        {
            User_id = id,
            Nome = nome,
            Email = email,
            Role = role
        };
    }
}
