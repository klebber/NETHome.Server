using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetHome.Data.Entities;
using NetHome.Data.Entities.Devices;

namespace NetHome.Data
{
    public class NetHomeContext : IdentityDbContext<User>
    {
        public DbSet<User> User { get; set; }
        public DbSet<Room> Room { get; set; }
        public DbSet<DeviceType> DeviceType { get; set; }
        public DbSet<Device> Device { get; set; }
        public DbSet<AirConditioner> AirConditioners { get; set; }
        public DbSet<DWSensor> DWSensor { get; set; }
        public DbSet<THSensor> THSensor { get; set; }
        public DbSet<RGBLight> RGBLight { get; set; }
        public DbSet<RollerShutter> RollerShutter { get; set; }
        public DbSet<SmartSwitch> SmartSwitch { get; set; }

        public NetHomeContext(DbContextOptions<NetHomeContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
