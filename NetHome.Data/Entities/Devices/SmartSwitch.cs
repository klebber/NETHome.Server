using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHome.Data.Entities.Devices
{
    public class SmartSwitch : Device
    {
        public bool Ison { get; set; }
        public override Uri ChangeState(Device newValue)
        {
            throw new NotImplementedException();
        }

        public override Uri RetrieveStateUri()
        {
            throw new NotImplementedException();
        }


        public override bool TryUpdateValues(Dictionary<string, string> values)
        {
            throw new NotImplementedException();
        }
    }
}
