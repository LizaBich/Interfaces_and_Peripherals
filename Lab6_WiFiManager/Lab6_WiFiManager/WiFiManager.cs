using SimpleWifi;
using System.Collections.Generic;
using System.Linq;

namespace Lab6_WiFiManager
{
    /// <summary>
    /// Subsidiary class 
    /// </summary>
    public static class WiFiManager
    {
        /// <summary>
        /// Object containing general information about wifi networks
        /// </summary>
        private static readonly Wifi Wifi = new Wifi();

        /// <summary>
        /// Used to get all available wifi networks
        /// </summary>
        /// <returns>Networks list</returns>
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
