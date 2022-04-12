using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetHome.Data.Entities.Devices
{
    public class RollerShutter : Device
    {
        public int CurrentPercentage { get; set; }
        public int FavPos1 { get; set; }
        public int FavPos2 { get; set; }
        public int FavPos3 { get; set; }
        public int FavPos4 { get; set; }

        public override Uri ChangeState(Device newValue)
        {
            RollerShutter rs = (RollerShutter)newValue;
            if (!ValidateValues(rs)) return null;
            CurrentPercentage = rs.CurrentPercentage;
            switch (Model)
            {
                case "Shelly 2.5":
                    var baseUri = new Uri($"http://{IpAdress}/roller/0");
                    var uri = new UriBuilder(baseUri);
                    string positionQuery = $"go=to_pos&roller_pos={rs.CurrentPercentage}";
                    uri.Query = positionQuery;
                    return uri.Uri;
                default:
                    throw new NotImplementedException();
            }
        }

        private static bool ValidateValues(RollerShutter roller) => roller.CurrentPercentage is >= 0 and <= 100;

        public override Uri RetrieveStateUri()
        {
            return new Uri($"http://{IpAdress}/roller/0");
        }

        public override bool TryUpdateValues(NameValueCollection values)
        {
            bool result = int.TryParse(values["current_pos"], out int newValue);
            if (result) CurrentPercentage = newValue;
            return result;
        }
    }
}
