using SimpleWifi;
using System.Collections.Generic;
using System.Linq;

namespace Lab6_WiFiManager
{
    public static class WiFiManager
    {
        private static readonly Wifi Wifi = new Wifi();

        public static List<WiFiNetwork> GetWiFiNetworks()
        {
            var accessPoints = Wifi.GetAccessPoints();
            return accessPoints.Select(point => new WiFiNetwork
                {
                    Name = point.Name,
                    Strength = point.SignalStrength + "%",
                    Description = point.ToString(),
                    IsSecure = point.IsSecure,
                    IsConnected = point.IsConnected
                })
                .ToList();
        }
    }
}
