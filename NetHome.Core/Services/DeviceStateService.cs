using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetHome.Common.Models;
using NetHome.Core.Exceptions;
using NetHome.Core.Helpers;
using NetHome.Data;
using NetHome.Data.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace NetHome.Core.Services
{
    public class DeviceStateService : IDeviceStateService
    {
        private readonly NetHomeContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private static readonly HttpClient client = new();

        public DeviceStateService(NetHomeContext context,
            UserManager<User> userManager,
            IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<DeviceModel> ChangeState(DeviceModel deviceModel, string userId)
        {
            await CheckExistenceAndAccess(deviceModel.Id, userId);
            var newValue = _mapper.Map<Device>(deviceModel);
            return ExecuteRequest(deviceModel.Id, newValue);
        }

        public async Task<DeviceModel> RefreshState(int deviceId, string userId)
        {
            await CheckExistenceAndAccess(deviceId, userId);
            return ExecuteRequest(deviceId);
        }

        public async Task StateChanged(string ip)
        {
            var deviceId = await _context.Device.Where(d => d.IpAdress == ip).Select(d => d.Id).SingleAsync();
            ExecuteRequest(deviceId);
        }
        public async Task StateChanged(string ip, NameValueCollection values)
        {
            var deviceId = await _context.Device.Where(d => d.IpAdress == ip).Select(d => d.Id).SingleAsync();
            ExecuteRequest(deviceId, values);
        }

        private async Task CheckExistenceAndAccess(int deviceId, string userId)
        {
            if (!_context.Device.Any(d => d.Id == deviceId))
                throw new InvalidOperationException("Requested device does not exist!");

            var roles = await _userManager.GetRolesAsync(await _userManager.FindByIdAsync(userId));
            if (roles.Any(r => r.Equals("Owner") || r.Equals("Admin")))
                return;

            if ((await _context.User
                    .Include(u => u.Devices)
                    .SingleAsync(u => u.Id == userId))
                    .Devices.ToList()
                    .Any(d => d.Id == deviceId))
                return;

            throw new AuthorizationException("You do not have access to this device!");
        }

        private DeviceModel ExecuteRequest(int deviceId)
        {
            lock (LockManager.GetDeviceLock(deviceId))
            {
                var device = _context.Device.Single(d => d.Id == deviceId);
                var uri = device.RetrieveStateUri();
                if (uri is null)
                    throw new SystemException("Unable to complete requested action!");
                if (uri.IsLoopback)
                    return _mapper.Map<DeviceModel>(device);
                if (HandleRetrieveStateRequest(uri, device))
                {
                    _context.SaveChanges();
                    return _mapper.Map<DeviceModel>(device);
                }
                throw new SystemException("Unable to complete requested action!");
            }
        }

        private DeviceModel ExecuteRequest(int deviceId, Device newValue)
        {
            lock (LockManager.GetDeviceLock(deviceId))
            {
                var device = _context.Device.Single(d => d.Id == deviceId);
                var uri = device.ChangeState(newValue);
                if (uri is not null && HandleChangeStateRequest(uri))
                {
                    _context.SaveChanges();
                    return _mapper.Map<DeviceModel>(device);
                }
                else
                {
                    throw new SystemException("Unable to complete requested action!");
                }
            }
        }

        private DeviceModel ExecuteRequest(int deviceId, NameValueCollection values)
        {
            lock (LockManager.GetDeviceLock(deviceId))
            {
                var device = _context.Device.Single(d => d.Id == deviceId);
                if (device.TryUpdateValues(values))
                {
                    _context.SaveChanges();
                    return _mapper.Map<DeviceModel>(device);
                }
                else
                {
                    throw new SystemException("Unable to complete requested action!");
                }
            }
        }

        private static bool HandleRetrieveStateRequest(Uri uri, Device device)
        {
            var result = client.Send(new HttpRequestMessage(HttpMethod.Get, uri));
            var json = result.Content.ReadAsStringAsync().Result;
            var values = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(json).ToNameValueCollection();
            return result.IsSuccessStatusCode is not false && device.TryUpdateValues(values);
        }

        private static bool HandleChangeStateRequest(Uri uri)
        {
            var result = client.Send(new HttpRequestMessage(HttpMethod.Get, uri));
            return result.IsSuccessStatusCode;
        }
    }
}
