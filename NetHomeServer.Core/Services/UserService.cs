using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NetHomeServer.Common.Models;
using NetHomeServer.Core.Exceptions;
using NetHomeServer.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHomeServer.Core.Services
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

        public async Task<LoginResponseModel> Login(LoginModel login)
        {
            var user = await _userManager.FindByNameAsync(login.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, login.Password)) 
                throw new AuthenticationException("Wrong username or password!");
            var token = _tokenService.GenerateToken(user);
            var userModel = _mapper.Map<UserModel>(user);
            userModel.Roles = await _userManager.GetRolesAsync(user);
            var loginResponseModel = new LoginResponseModel()
            {
                User = userModel,
                Token = token
            };
            return loginResponseModel;
        }

        public async Task Register(RegisterModel register)
        {
            if (await _userManager.FindByNameAsync(register.Username) != null)
                throw new ValidationException("Username already in use!");
            var user = _mapper.Map<User>(register);
            user.DateOfRegistration = DateTime.Now;
            var result = await _userManager.CreateAsync(user, register.Password);
            if(!result.Succeeded)
                throw new ValidationException("Unable to create account!");
        }

        public async Task<UserModel> Validate(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var userModel = _mapper.Map<UserModel>(user);
            userModel.Roles = await _userManager.GetRolesAsync(user);
            return userModel;
        }

        private async Task Initialize()
        {
            string[] roles = new string[] { "Owner", "Admin", "Tenant" };

            foreach (string role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            var result = await _userManager.GetUsersInRoleAsync("owner");

            if (result.Count == 0)
            {
                User user = new()
                {
                    UserName = "admin",
                    Email = "admin@home.net",
                    EmailConfirmed = true,
                    FirstName = "Ad",
                    LastName = "Min",
                    Age = 25,
                    Gender = "Male",
                    DateOfRegistration = DateTime.Now
                };
                await _userManager.CreateAsync(user, "admin");
                await _userManager.AddToRolesAsync(user, roles);
            }
        }
    }
}
