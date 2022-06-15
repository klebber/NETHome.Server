using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetHome.API.Helpers;
using NetHome.API.Middleware;
using NetHome.Core.Services;
using NetHome.Data;
using NetHome.Data.Entities;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddAuthentication(OptionsHelper.AuthenticationOptions()).AddJwtBearer(OptionsHelper.JwtBearerOptions(builder.Configuration["Jwt:Key"]));
services.AddAuthorization();
services.AddDbContext<NetHomeContext>(options => options.UseSqlite(builder.Configuration["ConnectionStrings:NetHomeContextConnection"]));
services.AddIdentityCore<User>(OptionsHelper.IdentityOptions()).AddRoles<IdentityRole>().AddEntityFrameworkStores<NetHomeContext>();
services.AddScoped<IHttpRequestHandler, HttpRequestHandler>();
services.AddSingleton<IWebSocketManager, WebSocketManager>();
services.AddScoped<IWebSocketHandler, WebSocketHandler>();
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddScoped<IUserService, UserService>();
services.AddScoped<ITokenService, TokenService>();
services.AddScoped<IDeviceService, DeviceService>();
services.AddScoped<IStateChangeNotifyService, StateChangeNotifyService>();
services.AddScoped<IDeviceStateService, DeviceStateService>();
services.AddControllers().AddJsonOptions(OptionsHelper.JsonOptions());

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