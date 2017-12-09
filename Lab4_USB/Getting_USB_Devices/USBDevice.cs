using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Getting_USB_Devices
{
    /// <summary>
    /// Class which describes usb-device
    /// </summary>
    public sealed class UsbDevice
    {
        /// <summary>
        /// Contains path to the RemoveDrive.exe
        /// </summary>
        private const string Path = "D:\\Downloads\\removedrive\\x64\\RemoveDrive.exe ";

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="name">Usb-device name</param>
        /// <param name="totalFreeSpace">Usb-device total free space</param>
        /// <param name="occupiedSpace">Space which occupied by data</param>
        /// <param name="totalSize">Usb-device total size</param>
        /// <param name="isMtp">Does usb-device support MTP (Media Transfer Protocol)</param>
        public UsbDevice(string name, string totalFreeSpace, string occupiedSpace, string totalSize, bool isMtp)
        {
            Name = name;
            TotalFreeSpace = totalFreeSpace;
            OccupiedSpace = occupiedSpace;
            TotalSize = totalSize;
            IsMtp = isMtp;
        }

        /// <summary>
        /// Usb-device name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Usb-device total free space 
        /// </summary>
        public string TotalFreeSpace { get; set; }

        /// <summary>
        /// Space which occupied by data
        /// </summary>
        public string OccupiedSpace { get; set; }

        /// <summary>
        /// Usb-device total size 
        /// </summary>
        public string TotalSize { get; set; }

        /// <summary>
        /// Does usb-device support MTP (Media Transfer Protocol)
        /// </summary>
        public bool IsMtp { get; set; }

        /// <summary>
        /// This method used to eject usb-device 
        /// </summary>
        /// <returns></returns>
        public string Eject()
        {
            List<string> output = new List<string>();
 
            var startInfo =
                new ProcessStartInfo(Path, Name.Remove(1))
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

            // Calling new process
            var ejectProcess =  Process.Start(startInfo);

            while (ejectProcess != null && !ejectProcess.StandardOutput.EndOfStream)
            {
                output.Add(ejectProcess.StandardOutput.ReadLine());
            }
            if (ejectProcess == null) return output[4] == "failed" ? "busy" : String.Empty;
            ejectProcess.WaitForExit();
            ejectProcess.Close();
            return output[4] == "failed" ? "busy" : String.Empty;
        }
    }
}
