using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHome.Data.Entities.Devices
{
    public class AirConditioner : Device
    {
        public bool Ison { get; set; }
        public int Temperature { get; set; }
        public int FanSpeed { get; set; }
        public bool Swing { get; set; }
        public bool TimerSet { get; set; }
        public double TimerValue { get; set; }

        public override Uri ChangeState(Device newValue)
        {
            AirConditioner air = (AirConditioner)newValue;
            return !ValidateValues(air)
                ? null
                : Model switch
                {
                    "diy" => GetDiyUriAndSetValues(air),
                    _ => throw new NotImplementedException(),
                };
        }

        private Uri GetDiyUriAndSetValues(AirConditioner air)
        {
            throw new NotImplementedException(); //TODO
        }

        private bool ValidateValues(AirConditioner air)
        {
            throw new NotImplementedException();//TODO
        }

        public override Uri RetrieveStateUri()
        {
            return new Uri("http://localhost");
        }

        public override bool TryUpdateValues(NameValueCollection values) => throw new InvalidOperationException();
    }
}
