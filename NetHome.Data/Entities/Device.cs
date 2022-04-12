using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHome.Data.Entities
{
    public abstract class Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public DateTime DateAdded { get; set; }
        public string IpAdress { get; set; }
        public string DeviceUsername { get; set; }
        public string DevicePassword { get; set; }
        public Room Room { get; set; }
        public DeviceType Type { get; set; }
        public ICollection<User> Users { get; set; }

        public abstract Uri ChangeState(Device newValue);
        public abstract Uri RetrieveStateUri();
        public abstract bool TryUpdateValues(NameValueCollection values);
    }
}
