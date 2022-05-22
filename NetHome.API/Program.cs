using System;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NetHome.API.Middleware;
using NetHome.Common.JsonConverters;
using NetHome.Common.Models;
using NetHome.Core.Exceptions;
using NetHome.Core.Services;
using NetHome.Data;
using NetHome.Data.Entities;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"])),
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
            var claims = new ClaimsIdentity();
            foreach (string role in await userManager.GetRolesAsync(user))
                claims.AddClaim(new Claim(ClaimTypes.Role, role));
            context.Principal.AddIdentity(claims);
        }
    };
});
services.AddAuthorization(options =>
{
    options.AddPolicy("ElevatedRights", policy => policy.RequireRole("Owner", "Admin"));
});
services.AddDbContext<NetHomeContext>(options => options.UseSqlite(builder.Configuration["ConnectionStrings:NetHomeContextConnection"]));
services.AddIdentityCore<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 4;
}).AddRoles<IdentityRole>()
  .AddEntityFrameworkStores<NetHomeContext>();
services.AddScoped<IHttpRequestHandler, HttpRequestHandler>();
services.AddSingleton<IWebSocketManager, WebSocketManager>();
services.AddScoped<IWebSocketHandler, WebSocketHandler>();
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddScoped<IUserService, UserService>();
services.AddScoped<ITokenService, TokenService>();
services.AddScoped<IDeviceService, DeviceService>();
services.AddScoped<IStateChangeNotifyService, StateChangeNotifyService>();
services.AddScoped<IDeviceStateService, DeviceStateService>();
services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.Converters.Add(new RuntimeTypeConverter<DeviceModel>("NetHome.Common.Models.Devices.", "NetHome.Common"));
});

var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseWebSockets(new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromSeconds(10)
});
app.UseMiddleware<WebSocketMiddleware>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();