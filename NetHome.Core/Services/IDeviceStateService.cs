using NetHome.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHome.Core.Services
{
    public interface IDeviceStateService
    {
        public Task<DeviceModel> ChangeState(DeviceModel deviceModel, string userId);
        public Task<DeviceModel> RefreshState(int deviceId, string userId);
        public void StateChanged(string ip);
        public Task StateChangedDW(string ip, string state);
        public Task StateChangedHT(string ip, int hum, double temp);
    }
}
