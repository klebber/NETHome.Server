using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetHome.API.Hubs
{
    [Authorize]
    public class NetHomeHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Debug.WriteLine("Someone has connected: " + Context.User.FindFirstValue(ClaimTypes.Name));
            return Task.CompletedTask;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Debug.WriteLine("Someone has LEFT: " + Context.User.FindFirstValue(ClaimTypes.Name));
            return base.OnDisconnectedAsync(exception);
        }

        public async Task Switch(bool ison)
        {
            string state = ison ? "on" : "off";
            Debug.WriteLine("Light has been switched " + state + " by: " + Context.User.FindFirstValue(ClaimTypes.Name));
            await Clients.All.SendAsync("Switched", ison);
        }
    }
}
