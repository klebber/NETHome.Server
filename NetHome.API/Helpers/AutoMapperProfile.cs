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

            CreateMap<Device, DeviceModel>()
                .ForMember(r => r.Room, opt => opt.MapFrom(src => src.Room.Name))
                .ForMember(t => t.Type, opt => opt.MapFrom(src => src.Type.Name));
            CreateMap<AirConditioner, AirConditionerModel>()
                .IncludeBase<Device, DeviceModel>();
            CreateMap<DWSensor, DWSensorModel>()
                .IncludeBase<Device, DeviceModel>();
            CreateMap<THSensor, THSensorModel>()
                .IncludeBase<Device, DeviceModel>();
            CreateMap<RGBLight, RGBLightModel>()
                .IncludeBase<Device, DeviceModel>();
            CreateMap<RollerShutter, RollerShutterModel>()
                .IncludeBase<Device, DeviceModel>();
            CreateMap<SmartSwitch, SmartSwitchModel>()
                .IncludeBase<Device, DeviceModel>();
        }
    }
}
