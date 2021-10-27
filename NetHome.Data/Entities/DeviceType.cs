using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHome.Data.Entities
{
    public class DeviceType
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Device> Devices { get; set; }
    }
}
