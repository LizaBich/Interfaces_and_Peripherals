using SimpleWifi;
using System.Linq;

namespace Lab6_WiFiManager
{
    public class WiFiNetwork
    {
        public string Name { get; set; }

        public string Strength { get; set; }

        public string Description { get; set; }

        public bool IsSecure { get; set; }

        public bool IsConnected { get; set; }

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
