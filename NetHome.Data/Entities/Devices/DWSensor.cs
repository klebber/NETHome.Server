using System;
using System.Collections.Specialized;

namespace NetHome.Data.Entities.Devices
{
    public class DWSensor : Device
    {
        public bool IsOpen { get; set; }
        public string Placement { get; set; }

        public override Uri ChangeState(Device newValue) => throw new InvalidOperationException();

        public override Uri RetrieveStateUri() => throw new InvalidOperationException();

        public override bool TryUpdateValues(NameValueCollection values)
        {
            bool result = bool.TryParse(values["state"], out bool newValue);
            if (result) IsOpen = newValue;
            return result;
        }
    }
}
