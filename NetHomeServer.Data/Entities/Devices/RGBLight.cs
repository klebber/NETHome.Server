using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHomeServer.Data.Entities.Devices
{
    public class RGBLight : Device
    {
        public bool Ison { get; set; }
        public string Mode { get; set; }
        public int Brightness { get; set; }
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
        public int White { get; set; }
        public int Gain { get; set; }
    }
}
