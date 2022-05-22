using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NetHome.Core.Helpers;

namespace NetHome.Core.Services
{
    public class WebSocketManager : IWebSocketManager
    {
        private readonly ConcurrentDictionary<string, WebSocketRecord> _socketRecords = new();

        private static string GenerateGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public WebSocketRecord GetSocketById(string id)
        {
            return _socketRecords.FirstOrDefault(p => p.Key == id).Value;
        }

        public WebSocketRecord GetSocketByUserId(string userId)
        {
            return _socketRecords.FirstOrDefault(p => p.Value.UserId == userId).Value;
        }

        public string GetId(WebSocketRecord socket)
        {
            return _socketRecords.FirstOrDefault(p => p.Value == socket).Key;
        }

        public string GetUserId(WebSocketRecord socket)
        {
            return _socketRecords.FirstOrDefault(p => p.Value == socket).Value.UserId;
        }

        public IEnumerable<WebSocketRecord> GetSockets()
        {
            return _socketRecords.Select(wsr => wsr.Value);
        }

        public IEnumerable<WebSocketRecord> GetSocketsByUserIds(IEnumerable<string> userIds)
        {
            return _socketRecords.Where(sr => userIds.Contains(sr.Value.UserId)).Select(k => k.Value);
        }

        public void AddSocket(WebSocketRecord socketRecord)
        {
            _socketRecords.TryAdd(GenerateGuid(), socketRecord);
        }

        public void RemoveSocket(WebSocketRecord socketRecord)
        {
            _socketRecords.TryRemove(GetId(socketRecord), out _);
        }
    }
}
