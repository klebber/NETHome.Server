using AutoMapper;
using NetHome.Common;
using NetHome.Common.Models;
using NetHome.Common.Models.Devices;
using NetHome.Data.Entities;
using NetHome.Data.Entities.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetHome.API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<RegisterRequest, User>();

            CreateMap<AirConditioner, AirConditionerModel>();
            CreateMap<DWSensor, DWSensorModel>();
            CreateMap<RGBLight, RGBLightModel>();
            CreateMap<RollerShutter, RollerShutterModel>();
            CreateMap<SmartSwitch, SmartSwitchModel>();
            CreateMap<Device, DeviceModel>()
                .Include<AirConditioner, AirConditionerModel>()
                .Include<DWSensor, DWSensorModel>()
                .Include<RGBLight, RGBLightModel>()
                .Include<RollerShutter, RollerShutterModel>()
                .Include<SmartSwitch, SmartSwitchModel>();
        }
    }
}
