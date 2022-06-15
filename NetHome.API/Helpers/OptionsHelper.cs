using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using NetHome.Core.Exceptions;
using Microsoft.AspNetCore.Identity;
using NetHome.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using NetHome.Common.JsonConverters;
using NetHome.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace NetHome.API.Helpers
{
    public static class OptionsHelper
    {
        public static Action<JwtBearerOptions> JwtBearerOptions(string key) => options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                RequireExpirationTime = true
            };
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    var id = context.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
                    var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();
                    var user = await userManager.FindByIdAsync(id) ?? throw new AuthorizationException("Unable to verify token!");
                    if (user.LockoutEnd > DateTime.Now)
                    {
                        var timeSpan = user.LockoutEnd - DateTime.Now;
                        var years = timeSpan.Value.Days / 365;
                        throw new AuthorizationException($"Your account has been locked for {years} years. Please contact an owner.");
                    }
                    var claims = new ClaimsIdentity();
                    foreach (string role in await userManager.GetRolesAsync(user))
                        claims.AddClaim(new Claim(ClaimTypes.Role, role));
                    context.Principal.AddIdentity(claims);
                }
            };
        };
        public static Action<AuthenticationOptions> AuthenticationOptions() => options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        };

        public static Action<IdentityOptions> IdentityOptions() => options =>
        {
            options.SignIn.RequireConfirmedAccount = false;
            options.Password.RequireDigit = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireLowercase = false;
            options.Password.RequiredLength = 4;
        };
        public static Action<JsonOptions> JsonOptions() => options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.Converters.Add(new RuntimeTypeConverter<DeviceModel>());
        };
    }
}
