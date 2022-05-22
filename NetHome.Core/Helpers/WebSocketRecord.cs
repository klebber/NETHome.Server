using System.Net.WebSockets;
using System.Threading;

namespace NetHome.Core.Helpers
{
    public record WebSocketRecord(string UserId, WebSocket Socket, CancellationTokenSource TokenSource);

}
