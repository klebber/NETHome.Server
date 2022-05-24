using System.Collections.Generic;
using System.Threading.Tasks;
using NetHome.Common;

namespace NetHome.Core.Services
{
    public interface IDeviceService
    {
        Task<DeviceModel> GetDevice(int deviceId, string userId);
        Task<ICollection<DeviceModel>> GetAllDevices(string userId);
        Task<DeviceModel> Add(DevicePayload devicePayload);
        Task<DeviceModel> Update(DevicePayload devicePayload);
        Task Delete(DeviceModel model);
    }
}
