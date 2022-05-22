using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetHome.Common;
using NetHome.Common.Models;
using NetHome.Core.Exceptions;
using NetHome.Data;
using NetHome.Data.Entities;

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

        public async Task<DeviceModel> Add(DevicePayload devicePayload)
        {
            ValidatePayload(devicePayload);
            var device = _mapper.Map<Device>(devicePayload.Device);
            device.DateAdded = DateTime.Now;
            SetPayloadValues(devicePayload, device);
            var result = _context.Add(device);
            await _context.SaveChangesAsync();
            return _mapper.Map<DeviceModel>(result);
        }

        public async Task<DeviceModel> Update(DevicePayload devicePayload)
        {
            ValidatePayload(devicePayload);
            var device = _context.Device.Single(d => d.Id == devicePayload.Device.Id);
            SetPayloadValues(devicePayload, device);
            await _context.SaveChangesAsync();
            return _mapper.Map<DeviceModel>(device);
        }

        public async Task Delete(DeviceModel model)
        {
            var device = _context.Device.Single(d => d.Id == model.Id);
            _context.Remove(device);
            await _context.SaveChangesAsync();
        }

        private void ValidatePayload(DevicePayload devicePayload)
        {
            if (!Uri.IsWellFormedUriString(devicePayload.IpAdress, UriKind.Absolute))
                throw new ValidationException("Invalid ip adress!");
            if (_context.Device.Any(d => d.Id != devicePayload.Device.Id && d.IpAdress == devicePayload.IpAdress))
                throw new ValidationException("Device with this ip adress exists!");
            if (string.IsNullOrWhiteSpace(devicePayload.Device.Name))
                throw new ValidationException("Invalid Name!");
            if (_context.Device.Any(d => d.Id != devicePayload.Device.Id && d.Name == devicePayload.Device.Name))
                throw new ValidationException("Device name is already in use!");
            if (string.IsNullOrWhiteSpace(devicePayload.Device.Model))
                throw new ValidationException("Invalid Model!");
            if (!_context.Room.Any(r => r.Name == devicePayload.Device.Room))
                throw new ValidationException("Invalid Room!");
            if (!_context.DeviceType.Any(t => t.Name == devicePayload.Device.Type))
                throw new ValidationException("Invalid Type!");
        }

        private void SetPayloadValues(DevicePayload devicePayload, Device device)
        {
            device.IpAdress = devicePayload.IpAdress;
            device.DeviceUsername = devicePayload.DeviceUsername;
            device.DevicePassword = devicePayload.DevicePassword;
            device.Room = _context.Room.Single(r => r.Name == device.Room.Name);
            device.Type = _context.DeviceType.Single(t => t.Name == device.Type.Name);
        }
    }
}
