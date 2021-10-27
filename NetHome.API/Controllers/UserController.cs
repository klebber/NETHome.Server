using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetHome.Common;
using NetHome.Common.Models;
using NetHome.Core.Services;
using NetHome.Data.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


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
            Debug.WriteLine("Login user: " + login.Username);
            return await _userService.Login(login);
        }

        [HttpPost("register")]
        public async Task Register([FromBody] RegisterRequest registerModel)
        {
            Debug.WriteLine("Register user: " + registerModel.Username);
            await _userService.Register(registerModel);
        }

        [Authorize]
        [HttpGet("validate")]
        public async Task<UserModel> Validate()
        {
            Debug.WriteLine("Validate user: " + User.Identity.Name);
            return await _userService.Validate(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }
}
