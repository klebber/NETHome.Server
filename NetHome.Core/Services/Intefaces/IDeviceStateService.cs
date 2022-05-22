using System.Collections.Specialized;
using System.Threading.Tasks;
using NetHome.Common.Models;

namespace NetHome.Core.Services
{
    public interface IDeviceStateService
    {
        Task<DeviceModel> ChangeState(DeviceModel deviceModel, string userId);
        Task<DeviceModel> RefreshState(int deviceId, string userId);
        Task StateChanged(string ip, NameValueCollection values = null);
    }
}
