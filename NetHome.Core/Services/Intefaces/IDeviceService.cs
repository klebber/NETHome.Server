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
        public Task<DeviceModel> GetDevice(int deviceId, string userId);
        public Task<ICollection<DeviceModel>> GetAllDevices(string userId);
    }
}
