﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHome.Data.Entities.Devices
{
    public class DWSensor : Device
    {
        public bool IsOpen { get; set; }
        public string Placement { get; set; }
    }
}
