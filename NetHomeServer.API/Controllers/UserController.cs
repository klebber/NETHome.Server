using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetHomeServer.Common.Models;
using NetHomeServer.Core.Services;
using NetHomeServer.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace NetHomeServer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            return Ok(await _userService.Login(login));
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            await _userService.Register(registerModel);
            return Ok();
        }

        [HttpGet("validate")]
        public async Task<IActionResult> Validate()
        {
            return Ok(await _userService.Validate(User.FindFirstValue(ClaimTypes.NameIdentifier)));
        }
    }
}
