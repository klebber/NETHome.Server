﻿using System.Collections.Generic;

namespace NetHome.Data.Entities
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Device> Devices { get; set; }
    }
}
