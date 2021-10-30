using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHome.Data.Entities.Devices
{
    public class THSensor : Device
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
    }
}
