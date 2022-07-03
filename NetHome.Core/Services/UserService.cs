using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using NetHome.Common;
using NetHome.Core.Exceptions;
using NetHome.Data;
using NetHome.Data.Entities;
using System.Collections.Generic;
using NetHome.Core.Helpers;
using Microsoft.EntityFrameworkCore;

namespace NetHome.Core.Services
{
    public class UserService : IUserService
    {
        private readonly NetHomeContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public UserService(NetHomeContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService,
            IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<LoginResponse> Login(LoginRequest login)
        {
            var user = await _userManager.FindByNameAsync(login.Username);
            if (user is null)
                throw new AuthorizationException("Wrong username or password!");
            CheckIfLocked(user);
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
            await _context.SaveChangesAsync();
            if (!result.Succeeded)
                throw new ValidationException("Unable to create account!");
        }

        public async Task<UserModel> Validate(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                throw new AuthorizationException("Wrong username or password!");
            CheckIfLocked(user);
            var userModel = _mapper.Map<UserModel>(user);
            userModel.Roles = await _userManager.GetRolesAsync(user);
            return userModel;
        }

        private static void CheckIfLocked(User user)
        {
            if (user.LockoutEnd > DateTime.Now)
            {
                var timeSpan = user.LockoutEnd - DateTime.Now;
                var years = timeSpan.Value.Days / 365;
                throw new AuthorizationException($"Your account has been locked for {years} years. Please contact an owner.");
            }
        }

        public async Task<UserModel> UpdateUser(UserModel userModel, string senderId)
        {
            bool isSenderAdmin = await _userManager.IsInRoleAsync(await _userManager.FindByIdAsync(senderId), "Owner");
            if (senderId != userModel.Id && !isSenderAdmin)
                throw new AuthorizationException("Owner role required for this action.");
            ValidateUserData(userModel);
            var user = await _userManager.FindByIdAsync(userModel.Id);
            SetUserValues(user, userModel);
            if (isSenderAdmin)
                await SetUserRoles(user, userModel);
            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();
            var resultModel = _mapper.Map<UserModel>(user);
            resultModel.Roles = await _userManager.GetRolesAsync(user);
            return resultModel;
        }

        private static void ValidateUserData(UserModel userModel)
        {
            if (userModel.Username.Length is < 4 or > 12) 
                throw new ValidationException("Username must be between 4 and 12 characters!");
            if (string.IsNullOrEmpty(userModel.FirstName))
                throw new ValidationException("You have not entered first name!");
            if (string.IsNullOrEmpty(userModel.LastName))
                throw new ValidationException("You have not entered last name!");
            if (!userModel.Email.IsValidEmail())
                throw new ValidationException("Email address is not valid!");
            if (userModel.Gender is not ("Male" or "Female" or "Other"))
                throw new ValidationException("Gender is invalid!");
            if (userModel.Age is <= 0 or >= 100)
                throw new ValidationException("Age is invalid!");
        }

        private static void SetUserValues(User user, UserModel userModel)
        {
            user.UserName = userModel.Username;
            user.FirstName = userModel.FirstName;
            user.LastName = userModel.LastName;
            user.Email = userModel.Email;
            user.Gender = userModel.Gender;
            user.Age = userModel.Age;
        }

        private async Task SetUserRoles(User user, UserModel userModel)
        {
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles.ToArray());
            await CheckRoles(userModel.Roles);
            await _userManager.AddToRolesAsync(user, userModel.Roles);
        }

        private async Task CheckRoles(ICollection<string> roles)
        {
            foreach (var role in roles)
                if (!await _roleManager.RoleExistsAsync(role))
                    throw new ArgumentException("Requested role does not exist!");
        }

        public async Task DeleteUser(string userId)
        {
            var user = await _context.User.Include(u => u.Devices).SingleAsync(u => u.Id == userId);
            if (await _userManager.IsInRoleAsync(user, "Owner"))
                throw new AuthorizationException("Unable to delete owner account.");
            user.Devices.Clear();
            await _userManager.DeleteAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task GiveUserDeviceAccess(DeviceAccessPayload deviceAccess)
        {
            var user = await _context.User.Include(u => u.Devices).SingleAsync(u => u.Id == deviceAccess.UserId);
            if (user.Devices.Any(d => d.Id == deviceAccess.DeviceId)) 
                return;
            var device = await _context.Device.SingleAsync(d => d.Id == deviceAccess.DeviceId);
            user.Devices.Add(device);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveUserDeviceAccess(DeviceAccessPayload deviceAccess)
        {
            var user = await _context.User.Include(u => u.Devices).SingleAsync(u => u.Id == deviceAccess.UserId);
            if (!user.Devices.Any(d => d.Id == deviceAccess.DeviceId))
                throw new ArgumentException("User does not have access to this device!");
            user.Devices.Remove(user.Devices.Single(d => d.Id == deviceAccess.DeviceId));
            await _context.SaveChangesAsync();
        }

        public async Task ChangePassword(PasswordChangePayload payload, string senderId)
        {
            if (!payload.UserId.Equals(senderId)) 
                throw new AuthorizationException("You can only change your own password!");
            var user = await _userManager.FindByIdAsync(payload.UserId);
            await _userManager.ChangePasswordAsync(user, payload.OldPassword, payload.NewPassword);
        }

        public async Task LockUser(UserModel userModel)
        {
            await SetUserLockTime(userModel, DateTime.MaxValue);
        }

        public async Task UnlockUser(UserModel userModel)
        {
            await SetUserLockTime(userModel, DateTime.Now);
        }

        private async Task SetUserLockTime(UserModel userModel, DateTime lockoutEnd)
        {
            var user = await _context.User.SingleAsync(u => u.Id == userModel.Id);
            if (await _userManager.IsInRoleAsync(user, "Owner"))
                throw new ValidationException("Unable to lock an owner.");
            await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);
            await _context.SaveChangesAsync();
        }
    }
}
