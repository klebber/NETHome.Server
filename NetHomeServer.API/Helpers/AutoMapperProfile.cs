using AutoMapper;
using NetHomeServer.Common.Models;
using NetHomeServer.Common.Models.Devices;
using NetHomeServer.Data.Entities;
using NetHomeServer.Data.Entities.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetHomeServer.API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<RegisterModel, User>();

            CreateMap<DeviceType, DeviceTypeModel>();
            CreateMap<Room, RoomModel>();
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
