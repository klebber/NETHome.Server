using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace NetHome.Data.Entities.Devices
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

        public override Uri ChangeState(Device newValue)
        {
            RGBLight rgb = (RGBLight)newValue;
            return !ValidateValues(rgb)
                ? null
                : Model switch
                {
                    "Shelly rgbw" => GetShellyUriAndSetValues(rgb),
                    _ => throw new NotImplementedException(),
                };
        }

        private Uri GetShellyUriAndSetValues(RGBLight rgb)
        {
            if (rgb.Mode != Mode)
            {
                Mode = rgb.Mode;
                return new Uri($"{IpAdress}/settings/0?mode={rgb.Mode}");
            }
            var baseUri = new Uri($"{IpAdress}/light/0");
            var query = new List<string>();
            if (Mode == "white")
            {
                if (Ison != rgb.Ison)
                {
                    Ison = rgb.Ison;
                    query.Add($"turn={(Ison ? "on" : "off")}");
                }
                if (Brightness != rgb.Brightness)
                {
                    Brightness = rgb.Brightness;
                    query.Add($"brightness={Brightness}");
                }
            }
            if (Mode == "color")
            {
                if (Ison != rgb.Ison)
                {
                    Ison = rgb.Ison;
                    query.Add($"turn={(Ison ? "on" : "off")}");
                }
                if (Gain != rgb.Gain)
                {
                    Gain = rgb.Gain;
                    query.Add($"gain={Gain}");
                }
                Red = rgb.Red;
                Green = rgb.Green;
                Blue = rgb.Blue;
                White = rgb.White;
                query.Add($"red={Red}");
                query.Add($"red={Green}");
                query.Add($"red={Blue}");
                query.Add($"red={White}");
            }
            var uriBuilder = new UriBuilder(baseUri)
            {
                Query = string.Join("&", query.Where(p => p.Length > 0))
            };
            return uriBuilder.Uri;
        }

        private static bool ValidateValues(RGBLight rgb) => rgb.Mode is not null
            && (rgb.Mode.Equals("color") || rgb.Mode.Equals("white"))
            && (rgb.Mode == "white" ? rgb.Brightness is >= 0 and <= 100
            : rgb.Red is >= 0 and <= 255 && rgb.Green is >= 0 and <= 255
            && rgb.Blue is >= 0 and <= 255 && rgb.White is >= 0 and <= 255
            && rgb.Gain is >= 0 and <= 100);

        public override Uri RetrieveStateUri() => new($"http://{IpAdress}/light/0");

        public override bool TryUpdateValues(NameValueCollection values)
        {
            try
            {
                Ison = bool.Parse(values["ison"]);
                Mode = values["mode"];
                Brightness = int.Parse(values["brightness"]);
                Red = int.Parse(values["red"]);
                Green = int.Parse(values["green"]);
                Blue = int.Parse(values["blue"]);
                White = int.Parse(values["white"]);
                Gain = int.Parse(values["gain"]);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
