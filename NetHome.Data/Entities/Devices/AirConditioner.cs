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
            if (!ValidateValues(air)) return null;
            SetValues(air);
            return Model switch
            {
                "diy" => new Uri($"http://{IpAdress}/ir?code={GenerateDiyCode()}"),
                _ => throw new NotImplementedException(),
            };
        }

        private void SetValues(AirConditioner air)
        {
            Ison = air.Ison;
            Temperature = air.Temperature;
            FanSpeed = air.FanSpeed;
            Swing = air.Swing;
            TimerSet = air.TimerSet;
            TimerValue = TimerSet ? air.TimerValue : 0;
        }

        private string GenerateDiyCode()
        {
            var array = new int[12];

            array[0] = 195;
            array[2] = 7;
            array[3] = 0;
            array[6] = 4;
            array[7] = 0;
            array[8] = 0;
            array[10] = 0;
            array[11] = 160;

            string tempBits = Convert.ToString(Temperature - 8, 2);
            tempBits = tempBits.PadLeft(5, '0');
            int tempReversedInt = Convert.ToInt32(new(tempBits.Reverse().ToArray()), 2);
            array[1] = (Swing ? 0 : 224) + tempReversedInt;

            int timerHours = (int)Math.Truncate(TimerValue);
            string timerBits = Convert.ToString(timerHours, 2);
            timerBits = timerBits.PadLeft(8, '0');
            string timerReversedBits = new(timerBits.Reverse().ToArray());
            int timerReversedInt = Convert.ToInt32(timerReversedBits, 2);
            int speedValue = FanSpeed switch
            {
                0 => 5,
                1 => 6,
                2 => 2,
                3 => 4,
                4 => 4,
                _ => throw new ArgumentOutOfRangeException()
            };
            array[4] = timerReversedInt + speedValue;

            array[5] = (TimerValue % 1 == 0 ? 0 : 120) + (speedValue == 5 ? 2 : 0);

            array[9] = (Ison ? 4 : 0) + (TimerSet ? 2 : 0);

            var bytes = array.Select(a => 
            {
                var s = Convert.ToString(a, 2);
                return s.PadLeft(8, '0');
            }).ToList();
            var reversedBytes = bytes.Select(x => new string(x.Reverse().ToArray())).ToList();
            var listInt = reversedBytes.Select(s => Convert.ToInt32(s, 2)).ToList();
            var sum = listInt.Sum();
            var binarySum = Convert.ToString(sum, 2);
            var invertedBinarySum = new string(binarySum.Reverse().ToArray());
            var checksum = invertedBinarySum[..8];

            return string.Join("", bytes) + checksum;
        }

        private static bool ValidateValues(AirConditioner air) =>
            air.FanSpeed is >= 0 and <= 4
            && air.TimerValue is >= 0 and <= 24
            && air.Temperature is >= 16 and <= 32
            && (air.TimerValue == Math.Truncate(air.TimerValue)
            || (air.TimerValue - Math.Truncate(air.TimerValue)) is 0.5);

        public override Uri RetrieveStateUri()
        {
            return new Uri("http://localhost");
        }

        public override bool TryUpdateValues(NameValueCollection values) => throw new InvalidOperationException();
    }
}
