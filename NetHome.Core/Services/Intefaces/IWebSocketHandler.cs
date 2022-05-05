using NetHome.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetHome.Core.Services
{
    public interface IWebSocketHandler
    {
        WebSocketRecord OnConnected(string id, WebSocket socket);
        Task OnDisconnected(WebSocketRecord socketRequest, string reason);
        Task SendAsync(WebSocketRecord socketRequest, string json);
        Task SendAsync(string id, string json);
        Task SendAsync(IEnumerable<string> ids, string json);
        Task ReceiveAsync(WebSocketRecord socketRequest);
    }
}
