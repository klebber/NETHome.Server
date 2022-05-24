using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NetHome.Common;
using NetHome.Core.Exceptions;
using NetHome.Data.Entities;

namespace NetHome.Core.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public UserService(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService,
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<LoginResponse> Login(LoginRequest login)
        {
            var user = await _userManager.FindByNameAsync(login.Username);
            if (user is null || !await _userManager.CheckPasswordAsync(user, login.Password))
                throw new AuthenticationException("Wrong username or password!");
            var token = _tokenService.GenerateToken(user);
            var userModel = _mapper.Map<UserModel>(user);
            userModel.Roles = await _userManager.GetRolesAsync(user);
            var loginResponseModel = new LoginResponse()
            {
                User = userModel,
                Token = token
            };
            return loginResponseModel;
        }

        public async Task Register(RegisterRequest register)
        {
            if (await _userManager.FindByNameAsync(register.Username) is not null)
                throw new ValidationException("Username already in use!");
            var user = _mapper.Map<User>(register);
            user.DateOfRegistration = DateTime.Now;
            var result = await _userManager.CreateAsync(user, register.Password);
            if (!result.Succeeded)
                throw new ValidationException("Unable to create account!");
        }

        public async Task<UserModel> Validate(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var userModel = _mapper.Map<UserModel>(user);
            userModel.Roles = await _userManager.GetRolesAsync(user);
            return userModel;
        }
    }
}
