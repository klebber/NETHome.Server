using System.Threading.Tasks;
using NetHome.Common.Models;

namespace NetHome.Core.Services
{
    public interface IStateChangeNotifyService
    {
        Task NotifyStateChangedAsync(DeviceModel device);
    }
}
