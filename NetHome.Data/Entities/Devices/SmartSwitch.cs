using System;
using System.Collections.Specialized;

namespace NetHome.Data.Entities.Devices
{
    public class SmartSwitch : Device
    {
        public bool Ison { get; set; }
        public override Uri ChangeState(Device newValue)
        {
            SmartSwitch ss = (SmartSwitch)newValue;
            Ison = ss.Ison;
            switch (Model)
            {
                case "Shelly 1":
                case "Shelly 1PM":
                    var baseUri = new Uri($"{IpAdress}/relay/0");
                    var uri = new UriBuilder(baseUri);
                    string isonQuery = $"turn={(Ison ? "on" : "off")}";
                    uri.Query = isonQuery;
                    return uri.Uri;
                default:
                    throw new NotImplementedException();
            }
        }

        public override Uri RetrieveStateUri() => new($"{IpAdress}/relay/0");


        public override bool TryUpdateValues(NameValueCollection values)
        {
            bool result = bool.TryParse(values["ison"], out bool newValue);
            if (result) Ison = newValue;
            return result;
        }
    }
}
