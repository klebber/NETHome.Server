using NetHome.Common.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHome.Core.Services
{
    public interface IDeviceStateService
    {
        public Task<DeviceModel> ChangeState(DeviceModel deviceModel, string userId);
        public Task<DeviceModel> RefreshState(int deviceId, string userId);
        public Task StateChanged(string ip, NameValueCollection values = null);
    }
}
