using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipMolde.Core.Interface.IUser;

namespace TipMolde.App.Controllers
{
    public class UserController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> SearchByName(string searchTerm)
        {
            var users = await _userService.SearchByNameAsync(searchTerm);
            return Ok(users);
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser(User user)
        {
            await _userService.CreateUserAsync(user);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser(User user)
        {
            await _userService.UpdateUserAsync(user);
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null || user.Password != password)
            {
                return Unauthorized();
            }
            // Implement token generation logic here
            return Ok(new { Token = "GeneratedToken" });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Implement logout logic here (e.g., invalidate token)
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(int userId, string currentPassword, string newPassword)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null || user.Password != currentPassword)
            {
                return Unauthorized();
            }
            user.Password = newPassword;
            await _userService.UpdateUserAsync(user);
            return Ok();
        }
}
