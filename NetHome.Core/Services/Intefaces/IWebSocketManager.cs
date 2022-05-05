using NetHome.Core.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace NetHome.Core.Services
{
    public interface IWebSocketManager
    {
        WebSocketRecord GetSocketById(string id);
        WebSocketRecord GetSocketByUserId(string userId);
        string GetId(WebSocketRecord socket);
        string GetUserId(WebSocketRecord socket);
        IEnumerable<WebSocketRecord> GetSockets();
        IEnumerable<WebSocketRecord> GetSocketsByUserIds(IEnumerable<string> userIds);
        void AddSocket(WebSocketRecord socket);
        void RemoveSocket(WebSocketRecord socket);
    }
}
