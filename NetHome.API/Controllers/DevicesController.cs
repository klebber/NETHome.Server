using System.Collections.Generic;
using System.Data;
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

        [Authorize(Roles = "Admin, Owner")]
        [HttpGet("getpayload")]
        public async Task<DevicePayload> GetPayload([FromQuery] int id)
        {
            return await _deviceService.GetDevicePayload(id);
        }

        [Authorize]
        [HttpGet("getall")]
        public async Task<ICollection<DeviceModel>> GetAll()
        {
            return await _deviceService.GetAllDevices(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [Authorize(Roles = "Admin, Owner")]
        [HttpPost("add")]
        public async Task<DeviceModel> Add([FromBody] DevicePayload device)
        {
            return await _deviceService.Add(device);
}

        [Authorize(Roles = "Admin, Owner")]
        [HttpPost("update")]
        public async Task<DeviceModel> Update([FromBody] DevicePayload device)
        {
            return await _deviceService.Update(device);
        }

        [Authorize(Roles = "Admin, Owner")]
        [HttpPost("delete")]
        public async Task Delete([FromBody] DeviceModel device)
        {
            await _deviceService.Delete(device);
        }

        [Authorize(Roles = "Admin, Owner")]
        [HttpGet("getrooms")]
        public async Task<ICollection<string>> GetRooms()
        {
            return await _deviceService.GetRooms();
        }

        [Authorize(Roles = "Admin, Owner")]
        [HttpGet("gettypes")]
        public async Task<ICollection<string>> GetDeviceTypes()
        {
            return await _deviceService.GetDeviceTypes();
        }

        [Authorize(Roles = "Admin, Owner")]
        [HttpPost("addroom")]
        public async Task AddRoom([FromBody] string roomName)
        {
            await _deviceService.AddRoom(roomName);
        }

        [Authorize(Roles = "Admin, Owner")]
        [HttpPost("deleteroom")]
        public async Task DeleteRoom([FromBody] string roomName)
        {
            await _deviceService.DeleteRoom(roomName);
        }

        [Authorize(Roles = "Admin, Owner")]
        [HttpPost("addtype")]
        public async Task AddDeviceType([FromBody] string typeName)
        {
            await _deviceService.AddType(typeName);
        }

        [Authorize(Roles = "Admin, Owner")]
        [HttpPost("deletetype")]
        public async Task DeleteDeviceType([FromBody] string typeName)
        {
            await _deviceService.DeleteType(typeName);
        }
    }
}
