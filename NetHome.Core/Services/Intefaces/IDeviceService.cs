using System.Collections.Generic;
using System.Threading.Tasks;
using NetHome.Common;

namespace NetHome.Core.Services
{
    public interface IDeviceService
    {
        Task<DeviceModel> GetDevice(int deviceId, string userId);
        Task<DevicePayload> GetDevicePayload(int id);
        Task<ICollection<DeviceModel>> GetAllDevices(string userId);
        Task<DeviceModel> Add(DevicePayload devicePayload);
        Task<DeviceModel> Update(DevicePayload devicePayload);
        Task Delete(DeviceModel model);
        Task<ICollection<string>> GetRooms();
        Task<ICollection<string>> GetDeviceTypes();
        Task AddRoom(string roomName);
        Task DeleteRoom(string roomName);
        Task AddType(string typeName);
        Task DeleteType(string typeName);
    }
}
