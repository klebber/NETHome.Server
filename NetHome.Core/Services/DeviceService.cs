using AutoMapper;
using NetHome.Common.Models;
using NetHome.Data;
using NetHome.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetHome.Data.Entities.Devices;
using NetHome.Core.Exceptions;
using Microsoft.AspNetCore.Identity;
using System.Collections.Concurrent;

namespace NetHome.Core.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly NetHomeContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public DeviceService(NetHomeContext context,
            UserManager<User> userManager,
            IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<DeviceModel> GetDevice(int deviceId, string userId)
        {
            var device = await _context.Device.SingleAsync(d => d.Id == deviceId);
            await CheckDeviceAccess(deviceId, userId);
            var devicemodel = _mapper.Map<DeviceModel>(device);
            return devicemodel;
        }

        public async Task<ICollection<DeviceModel>> GetAllDevices(string userId)
        {
            return await CheckElevatedAccess(userId)
                ? await GetAllDevices()
                : await GetDevicesForUser(userId);
        }

        private async Task<ICollection<DeviceModel>> GetAllDevices()
        {
            var devices = await _context.Device
                .Include(d => d.Room)
                .Include(d => d.Type)
                .OrderBy(d => d.Room.Id)
                .ThenBy(d => d.Type.Id)
                .ToListAsync();
            var devicemodels = _mapper.Map<List<Device>, List<DeviceModel>>(devices);
            return devicemodels;
        }

        private async Task<ICollection<DeviceModel>> GetDevicesForUser(string userId)
        {
            var user = await _context.User
                .Include(u => u.Devices)
                .SingleAsync(u => u.Id == userId);
            var devices = user.Devices
                .OrderBy(d => d.Room.Id)
                .ThenBy(d => d.Type.Id)
                .ToList();
            var devicemodels = _mapper.Map<List<Device>, List<DeviceModel>>(devices);
            return devicemodels;
        }

        private async Task CheckDeviceAccess(int deviceId, string userId)
        {
            if (await CheckElevatedAccess(userId))
                return;
            if (_context.User
                .Include(u => u.Devices)
                .Single(u => u.Id == userId)
                .Devices.ToList()
                .Any(d => d.Id == deviceId))
                return;
            throw new AuthorizationException("You do not have access to this device!");
        }

        private async Task<bool> CheckElevatedAccess(string userId)
        {
            var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(userId));
            return roles.Any(r => r.Equals("Owner") || r.Equals("Admin"));
        }

    }
}
