using NetHome.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHome.Core.Services
{
    public interface IDeviceService
    {
        public DeviceModel GetDevice(int deviceId, string userId, bool elevatedRights);
        public ICollection<DeviceModel> GetAllDevices();
        public ICollection<DeviceModel> GetDevicesForUser(string userId);
    }
}
