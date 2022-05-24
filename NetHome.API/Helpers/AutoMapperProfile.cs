using AutoMapper;
using NetHome.Common;
using NetHome.Data.Entities;
using NetHome.Data.Entities.Devices;

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

            CreateMap<DeviceModel, Device>()
                .ForMember(r => r.Room, opt => opt.MapFrom(src => new Room() { Id = -1, Name = src.Room }))
                .ForMember(t => t.Type, opt => opt.MapFrom(src => new DeviceType() { Id = -1, Name = src.Type }));
            CreateMap<AirConditionerModel, AirConditioner>()
                .IncludeBase<DeviceModel, Device>();
            CreateMap<DWSensorModel, DWSensor>()
                .IncludeBase<DeviceModel, Device>();
            CreateMap<THSensorModel, THSensor>()
                .IncludeBase<DeviceModel, Device>();
            CreateMap<RGBLightModel, RGBLight>()
                .IncludeBase<DeviceModel, Device>();
            CreateMap<RollerShutterModel, RollerShutter>()
                .IncludeBase<DeviceModel, Device>();
            CreateMap<SmartSwitchModel, SmartSwitch>()
                .IncludeBase<DeviceModel, Device>();
        }
    }
}
