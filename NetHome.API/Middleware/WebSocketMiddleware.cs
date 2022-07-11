using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NetHome.Core.Exceptions;
using NetHome.Core.Services;

namespace NetHome.API.Middleware
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;

        public WebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IWebSocketHandler _webSocketHandler)
        {
            if (context.WebSockets.IsWebSocketRequest && context.User.Identity.IsAuthenticated)
            {
                using var socket = await context.WebSockets.AcceptWebSocketAsync();
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                if (userId is null) throw new AuthenticationException("WebSocket authentication error!");
                var wsr = _webSocketHandler.OnConnected(userId, socket);
                await _webSocketHandler.ReceiveAsync(wsr);
            }
            else
            {
                await _next(context);
            }
        }
    }
}
