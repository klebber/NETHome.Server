using System.Collections.Generic;
using NetHome.Core.Helpers;

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
