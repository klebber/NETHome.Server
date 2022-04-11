using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetHome.Common.Models;
using NetHome.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
        public async Task Add([FromBody] DeviceModel device)
        {
            throw new NotImplementedException();
        }
    }
}
