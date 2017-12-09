using System.Linq;
using System.Management;

namespace Lab5_DeviceManager
{
    /// <summary>
    /// Class that describes device
    /// </summary>
    public class Device
    {
        /// <summary>
        /// Device name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Device guid
        /// </summary>
        public string ClassGuid { get; set; }

        /// <summary>
        /// Hardware Id of device
        /// </summary>
        public string HardwareId { get; set; }

        /// <summary>
        /// Device manufacturer
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// Device description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Device provider
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// System path
        /// </summary>
        public string SysPath { get; set; }

        /// <summary>
        /// Device path
        /// </summary>
        public string DevicePath { get; set; }

        /// <summary>
        /// Device status
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Method that changes connection 
        /// </summary>
        /// <param name="method">Command "Disable" or "Enable"</param>
        public void ChangeConnection(string method)
        {
            var device = new ManagementObjectSearcher("SELECT * FROM Win32_PNPEntity").Get().OfType<ManagementObject>()
                .FirstOrDefault(x => x["DeviceID"].ToString().Equals(DevicePath));
            device?.InvokeMethod(method, new object[] { false });
        }
    }
}
