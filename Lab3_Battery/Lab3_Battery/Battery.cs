using System;
using System.Globalization;
using System.Windows.Forms;

namespace Lab3_Battery
{
    class Battery
    {
        public string AvailPower { get; set; }
        public int AvailTime { get; set; }
        public string ConnectType { get; set; }

        public string GetConnectType()
        {
            ConnectType = SystemInformation.PowerStatus.PowerLineStatus.ToString();
            switch (ConnectType)
            {
                case "Online": return "Battery";
                case "Offline": return "AC";
                default: return "Unknown";
            }
        }

        public string GetAvailTime()
        {
            AvailTime = SystemInformation.PowerStatus.BatteryLifeRemaining;
            if (AvailTime != -1 && ConnectType == "Offline")
            {
                return new TimeSpan(0, AvailTime / 60, 0).ToString();
            }
            if (AvailTime == -1 && ConnectType == "Online")
            {
                return "время не ограничено.";
            }
            return "Unknown";
        }

        public string GetAvailPower()
        {
            AvailPower = Convert.ToString(SystemInformation.PowerStatus.BatteryLifePercent * 100, CultureInfo.CurrentCulture) + "%";
            return AvailPower;
        }
    }
}
