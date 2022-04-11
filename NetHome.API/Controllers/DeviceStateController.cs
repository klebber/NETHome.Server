using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetHome.API.Helpers;
using NetHome.Common.Models;
using NetHome.Core.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetHome.API.Controllers
{
    [Route("api/state")]
    [ApiController]
    public class DeviceStateController : ControllerBase
    {
        private readonly IDeviceStateService _deviceStateService;
        public DeviceStateController(IDeviceStateService deviceStateService)
        {
            _deviceStateService = deviceStateService;
        }
        [Authorize]
        [HttpPost("change")]
        public async Task<DeviceModel> ChangeDeviceState([FromBody] DeviceModel device)
        {
            return await _deviceStateService.ChangeState(device, User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [Authorize]
        [HttpGet("refresh")]
        public async Task<DeviceModel> RefreshDeviceState([FromQuery] int deviceId)
        {
            return await _deviceStateService.RefreshState(deviceId, User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [LocalAddress]
        [HttpGet("report")]
        public void ReportStateChanged()
        {
            _deviceStateService.StateChanged(GetClientIP());
        }

        private string GetClientIP() => Request.HttpContext.Connection.RemoteIpAddress.ToString();
    }
}
