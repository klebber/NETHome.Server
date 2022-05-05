using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetHome.Common.JsonConverters;
using NetHome.Common.Models;
using NetHome.Data;
using NetHome.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NetHome.Core.Services
{
    public class StateChangeNotifyService : IStateChangeNotifyService
    {
        private readonly NetHomeContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebSocketHandler _handler;

        public StateChangeNotifyService(NetHomeContext context,
            UserManager<User> userManager,
            IWebSocketHandler webSocketHandler)
        {
            _context = context;
            _userManager = userManager;
            _handler = webSocketHandler;
        }

        public async Task NotifyStateChangedAsync(DeviceModel device)
        {
            var ids = await GetUsersWithAccess(device);
            await ExecuteAction(device, ids);
        }

        private async Task<IEnumerable<string>> GetUsersWithAccess(DeviceModel device)
        {
            var owners = (await _userManager.GetUsersInRoleAsync("Owner")).Select(u => u.Id);
            var admins = (await _userManager.GetUsersInRoleAsync("Admin")).Select(u => u.Id);
            var elevated = owners.Union(admins).Distinct();
            var others = (await _context.Device.Include(d => d.Users).SingleAsync(d => d.Id == device.Id)).Users.Select(u => u.Id);
            return elevated.Union(others).Distinct();
        }

        private async Task ExecuteAction(DeviceModel device, IEnumerable<string> ids)
        {
            if (ids == null || !ids.Any())
                return;
            JsonSerializerOptions options = new();
            options.PropertyNameCaseInsensitive = true;
            options.Converters.Add(new RuntimeTypeConverter<DeviceModel>("NetHome.Common.Models.Devices.", "NetHome.Common"));
            string json = JsonSerializer.Serialize(device, options);
            await _handler.SendAsync(ids, json);
        }
    }
}
