using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetHome.Common;
using NetHome.Core.Services;

namespace NetHome.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevicesController : ControllerBase
    {
        private readonly IDeviceService _deviceService;
        public DevicesController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [Authorize]
        [HttpGet("get")]
        public async Task<DeviceModel> Get([FromQuery] int id)
        {
            return await _deviceService.GetDevice(id, User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [Authorize]
        [HttpGet("getall")]
        public async Task<ICollection<DeviceModel>> GetAll()
        {
            return await _deviceService.GetAllDevices(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [Authorize(Policy = "ElevatedRights")]
        [HttpPost("add")]
        public async Task<DeviceModel> Add([FromBody] DevicePayload device)
        {
            return await _deviceService.Add(device);
        }

        [Authorize(Policy = "ElevatedRights")]
        [HttpPost("update")]
        public async Task<DeviceModel> Update([FromBody] DevicePayload device)
        {
            return await _deviceService.Update(device);
        }

        [Authorize(Policy = "ElevatedRights")]
        [HttpPost("delete")]
        public async void Delete([FromBody] DeviceModel device)
        {
            await _deviceService.Delete(device);
        }
    }
}
