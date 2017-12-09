using SimpleWifi;
using System.Linq;

namespace Lab6_WiFiManager
{
    /// <summary>
    /// Class that describes wi-fi network
    /// </summary>
    public class WiFiNetwork
    {
        /// <summary>
        /// Network name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The strength of the signal
        /// </summary>
        public string Strength { get; set; }

        /// <summary>
        /// The description of the network
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// True when network protected by password
        /// </summary>
        public bool IsSecure { get; set; }

        /// <summary>
        /// True when connection existing
        /// </summary>
        public bool IsConnected { get; set; }

        /// <summary>
        /// Used to connect to the network
        /// </summary>
        /// <param name="password">Password</param>
        /// <returns>true - success, false - not success</returns>
        public bool Connect(string password)
        {
            var wifi = new Wifi();
            var accessPoint = wifi.GetAccessPoints().FirstOrDefault(point => point.Name.Equals(Name));
            if (accessPoint == null) return false;

            var request = new AuthRequest(accessPoint)
            {
                Password = password
            };
            
            return accessPoint.Connect(request);
        }
    }
}
