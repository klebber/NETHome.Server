using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHomeServer.Common.Models
{
    public abstract class DeviceModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public DateTime DateAdded { get; set; }
        public RoomModel Room { get; set; }
        public DeviceTypeModel Type { get; set; }
    }
}
