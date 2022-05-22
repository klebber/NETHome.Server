using System;
using System.Collections.Specialized;

namespace NetHome.Data.Entities.Devices
{
    public class THSensor : Device
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }

        public override Uri ChangeState(Device newValue) => throw new InvalidOperationException();

        public override Uri RetrieveStateUri() => throw new InvalidOperationException();

        public override bool TryUpdateValues(NameValueCollection values)
        {
            try
            {
                Temperature = double.Parse(values["temp"]);
                Humidity = double.Parse(values["hum"]);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
