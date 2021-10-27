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
        [HttpGet("get/{id}")]
        public DeviceModel Get(int id)
        {
            return _deviceService.GetDevice(id, User.FindFirstValue(ClaimTypes.NameIdentifier), User.IsInRole("Admin") || User.IsInRole("Owner"));
        }
        
        [Authorize]
        [HttpGet("getall")]
        public ICollection<DeviceModel> GetAll()
        {
            Debug.WriteLine("GetAll request from user: " + User.Identity.Name);
            return User.IsInRole("Admin") || User.IsInRole("Owner")
                ? _deviceService.GetAllDevices()
                : _deviceService.GetDevicesForUser(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [Authorize(Policy = "ElevatedRights")]
        [HttpPost("add")]
        public void Add(DeviceModel device)
        {
            throw new NotImplementedException();
        }
    }
}
