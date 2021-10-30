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

namespace NetHome.Core.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly NetHomeContext _context;
        private readonly IMapper _mapper;
        public DeviceService(NetHomeContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public DeviceModel GetDevice(int deviceId, string userId, bool elevatedRights)
        {
            var device = _context.Device.SingleOrDefault(d => d.Id == deviceId);
            if (device is null) throw new Exception("Device not found.");
            if (!elevatedRights && !HasDeviceAccess(deviceId, userId))
                throw new AuthorizationException("You do not have access to this device!"); 
            var devicemodel = _mapper.Map<DeviceModel>(device);
            return devicemodel;
        }

        private bool HasDeviceAccess(int deviceId, string userId)
        {
            return _context.User
                .Include(u => u.Devices)
                .Single(u => u.Id == userId)
                .Devices.ToList()
                .Any(d => d.Id == deviceId);
        }

        public ICollection<DeviceModel> GetAllDevices()
        {
            var devices = _context.Device
                .Include(d => d.Room)
                .Include(d => d.Type)
                .OrderBy(d => d.Room.Id)
                .ThenBy(d => d.Type.Id)
                .ToList();
            var devicemodels = _mapper.Map<List<Device>, List<DeviceModel>>(devices);
            return devicemodels;
        }

        public ICollection<DeviceModel> GetDevicesForUser(string userId)
        {
            var devices = _context.User
                .Include(u => u.Devices)
                .Single(u => u.Id == userId)
                .Devices
                .OrderBy(d => d.Room.Id)
                .ThenBy(d => d.Type.Id)
                .ToList();
            var devicemodels = _mapper.Map<List<Device>, List<DeviceModel>>(devices);
            return devicemodels;
        }
    }
}
