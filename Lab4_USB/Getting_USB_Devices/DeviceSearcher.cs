using MediaDevices;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Getting_USB_Devices
{
    /// <summary>
    /// Auxiliary class. Gets all usb-devices
    /// </summary>
    public static class DeviceSearcher
    {
        /// <summary>
        /// Method which gets all usb-devices
        /// </summary>
        /// <returns>List with usb-devices</returns>
        public static List<UsbDevice> GetDevices()
        {
            var devices = new List<UsbDevice>();
            var drives = DriveInfo.GetDrives().Where(d => d.IsReady && d.DriveType == DriveType.Removable).ToList();
            var mtpDevices = MediaDevice.GetDevices().ToList();
            foreach (var device in mtpDevices)
            {
                device.Connect();
                if (device.DeviceType != DeviceType.Generic)
                {
                    devices.Add(new UsbDevice(device.Manufacturer + " " + device.Model, null, null, null, true));
                }
            }
            devices.AddRange(drives.Select(drive => new UsbDevice(drive.Name, 
                BytesToMegaBytesString(drive.TotalFreeSpace), 
                BytesToMegaBytesString(drive.TotalSize - drive.TotalFreeSpace), 
                BytesToMegaBytesString(drive.TotalSize), 
                false)));
            return devices;
        }

        /// <summary>
        /// Converts bytes to megabytes and returns converted to string result 
        /// </summary>
        /// <param name="value">Bytes</param>
        /// <returns>string with result of conversion</returns>
        private static string BytesToMegaBytesString(long value) => value / 1024 / 1024 + " MB";
    }
}
