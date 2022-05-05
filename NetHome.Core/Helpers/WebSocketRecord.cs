using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetHome.Core.Helpers
{
    public record WebSocketRecord(string UserId, WebSocket Socket, CancellationTokenSource TokenSource);

}
