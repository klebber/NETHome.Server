using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NetHome.Common.Models;
using NetHome.Core.Exceptions;
using NetHome.Core.Helpers;
using NetHome.Data;
using NetHome.Data.Entities;

namespace NetHome.Core.Services
{
    public class DeviceStateService : IDeviceStateService
    {
        private readonly NetHomeContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IStateChangeNotifyService _changeNotifyService;
        private readonly IHttpRequestHandler _httpHandler;

        public DeviceStateService(NetHomeContext context,
            UserManager<User> userManager,
            IMapper mapper,
            IStateChangeNotifyService changeNotifyService,
            IHttpRequestHandler httpHandler)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _changeNotifyService = changeNotifyService;
            _httpHandler = httpHandler;
        }

        public async Task<DeviceModel> ChangeState(DeviceModel deviceModel, string userId)
        {
            await CheckExistenceAndAccess(deviceModel.Id, userId);
            var newValue = _mapper.Map<Device>(deviceModel);
            var device = ExecuteStateChange(deviceModel.Id, newValue);
            await _changeNotifyService.NotifyStateChangedAsync(device);
            return device;
        }

        public async Task<DeviceModel> RefreshState(int deviceId, string userId)
        {
            await CheckExistenceAndAccess(deviceId, userId);
            var device = ExecuteStateFetchAndUpdate(deviceId);
            await _changeNotifyService.NotifyStateChangedAsync(device);
            return device;
        }

        public async Task StateChanged(string ip, NameValueCollection values = null)
        {
            var deviceId = await _context.Device.Include(d => d.Room).Include(d => d.Type).Where(d => d.IpAdress == ip).Select(d => d.Id).SingleAsync();
            var device = values is null ? ExecuteStateFetchAndUpdate(deviceId) : ExecuteStateUpdate(deviceId, values);
            await _changeNotifyService.NotifyStateChangedAsync(device);
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

        private DeviceModel ExecuteStateChange(int deviceId, Device newValue)
        {
            lock (LockManager.GetDeviceLock(deviceId))
            {
                var device = _context.Device.Include(d => d.Room).Include(d => d.Type).Single(d => d.Id == deviceId);
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

        private DeviceModel ExecuteStateFetchAndUpdate(int deviceId)
        {
            lock (LockManager.GetDeviceLock(deviceId))
            {
                var device = _context.Device.Include(d => d.Room).Include(d => d.Type).Single(d => d.Id == deviceId);
                var uri = device.RetrieveStateUri();
                if (uri is not null)
                {
                    if (uri.IsLoopback)
                        return _mapper.Map<DeviceModel>(device);
                    if (HandleRetrieveStateRequest(uri, device))
                    {
                        _context.SaveChanges();
                        return _mapper.Map<DeviceModel>(device);
                    }
                }
                throw new SystemException("Unable to complete requested action!");
            }
        }

        private DeviceModel ExecuteStateUpdate(int deviceId, NameValueCollection values)
        {
            lock (LockManager.GetDeviceLock(deviceId))
            {
                var device = _context.Device.Include(d => d.Room).Include(d => d.Type).Single(d => d.Id == deviceId);
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

        private bool HandleRetrieveStateRequest(Uri uri, Device device)
        {
            var result = _httpHandler.Get(uri);
            var json = result.Content.ReadAsStringAsync().Result;
            var values = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(json).ToNameValueCollection();
            return result.IsSuccessStatusCode is not false && device.TryUpdateValues(values);
        }

        private bool HandleChangeStateRequest(Uri uri)
        {
            try
            {
                var result = _httpHandler.Get(uri);
                return result.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.StackTrace);
            }
            return false;
        }
    }
}
