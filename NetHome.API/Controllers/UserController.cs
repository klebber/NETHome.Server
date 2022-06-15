using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetHome.API.Helpers;
using NetHome.Common;
using NetHome.Core.Services;


namespace NetHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<LoginResponse> Login([FromBody] LoginRequest login)
        {
            return await _userService.Login(login);
        }

        [LocalAddress]
        [HttpPost("register")]
        public async Task Register([FromBody] RegisterRequest registerModel)
        {
            await _userService.Register(registerModel);
        }

        [Authorize]
        [HttpGet("validate")]
        public async Task<UserModel> Validate()
        {
            return await _userService.Validate(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [Authorize]
        [HttpPost("update")]
        public async Task<UserModel> Update([FromBody] UserModel userModel)
        {
            return await _userService.UpdateUser(userModel, User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [Authorize(Roles = "Owner")]
        [HttpPost("delete")]
        public async Task Delete([FromBody] string userId)
        {
            await _userService.DeleteUser(userId);
        }

        [Authorize(Roles = "Owner")]
        [HttpPost("giveaccess")]
        public async Task GiveAccess([FromBody] DeviceAccessPayload deviceAccess)
        {
            await _userService.GiveUserDeviceAccess(deviceAccess);
        }

        [Authorize(Roles = "Owner")]
        [HttpPost("removeaccess")]
        public async Task RemoveAccess([FromBody] DeviceAccessPayload deviceAccess)
        {
            await _userService.RemoveUserDeviceAccess(deviceAccess);
        }

        [Authorize]
        [HttpPost("changepassword")]
        public async Task ChangePassword([FromBody] PasswordChangePayload payload)
        {
            await _userService.ChangePassword(payload, User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [Authorize(Roles = "Owner")]
        [HttpPost("lock")]
        public async Task LockUser([FromBody] UserModel user)
        {
            await _userService.LockUser(user);
        }

        [Authorize(Roles = "Owner")]
        [HttpPost("unlock")]
        public async Task UnlockUser([FromBody] UserModel user)
        {
            await _userService.UnlockUser(user);
        }
    }
}
