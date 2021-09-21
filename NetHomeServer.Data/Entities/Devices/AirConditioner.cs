using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHomeServer.Data.Entities.Devices
{
    public class AirConditioner : Device
    {
        public bool Ison { get; set; }
        public int Temperature { get; set; }
        public int FanSpeed { get; set; }
        public bool Swing { get; set; }
        public bool TimerSet { get; set; }
        public double TimerValue { get; set; }
    }
}
