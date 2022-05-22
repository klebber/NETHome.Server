using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetHome.Core.Helpers;

namespace NetHome.Core.Services
{
    public class WebSocketHandler : IWebSocketHandler
    {
        private readonly IWebSocketManager _manager;

        public WebSocketHandler(IWebSocketManager manager)
        {
            _manager = manager;
        }

        public WebSocketRecord OnConnected(string userId, WebSocket socket)
        {
            var wsr = new WebSocketRecord(userId, socket, new CancellationTokenSource());
            _manager.AddSocket(wsr);
            return wsr;
        }

        public async Task OnDisconnected(WebSocketRecord socketRecord, string reason = "")
        {
            if (socketRecord.Socket.State is WebSocketState.Open or WebSocketState.CloseReceived)
                await socketRecord.Socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, reason, CancellationToken.None);
            else if (!socketRecord.TokenSource.IsCancellationRequested)
                socketRecord.TokenSource.Cancel();
            _manager.RemoveSocket(socketRecord);
        }

        public async Task SendAsync(WebSocketRecord socketRequest, string json)
        {
            if (socketRequest.Socket.State != WebSocketState.Open)
            {
                await OnDisconnected(socketRequest);
                return;
            }
            var buffer = new ArraySegment<byte>(Encoding.ASCII.GetBytes(json), 0, json.Length);
            await socketRequest.Socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task SendAsync(string userId, string json)
        {
            await SendAsync(_manager.GetSocketByUserId(userId), json);
        }

        public async Task SendAsync(IEnumerable<string> ids, string json)
        {
            var sockets = _manager.GetSocketsByUserIds(ids);
            foreach (var socket in sockets)
                await SendAsync(socket, json);
        }

        public async Task ReceiveAsync(WebSocketRecord socketRequest)
        {
            try
            {
                var buffer = new byte[1024 * 4];

                while (socketRequest.Socket.State == WebSocketState.Open)
                {
                    var result = await socketRequest.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await OnDisconnected(socketRequest);
                    }
                }
            }
            catch (WebSocketException)
            {
                await OnDisconnected(socketRequest);
            }
            catch (OperationCanceledException)
            {
                await OnDisconnected(socketRequest);
            }
            finally
            {
                socketRequest.Socket.Dispose();
                socketRequest.TokenSource.Dispose();
            }
        }
    }
}
