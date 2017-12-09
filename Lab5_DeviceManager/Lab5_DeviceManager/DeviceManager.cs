﻿using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace Lab5_DeviceManager
{
    /// <summary>
    /// Class that gets list of devices
    /// </summary>
    public class DeviceManager
    {
        /// <summary>
        /// 
        /// </summary>
        public ManagementObjectSearcher Searcher = new ManagementObjectSearcher(new ManagementScope("\\\\.\\root\\cimv2"), 
            new SelectQuery("SELECT * FROM Win32_PnPEntity"));

        /// <summary>
        /// Method that gets list of devices
        /// </summary>
        /// <returns>List of devices</returns>
        public List<Device> GetDevices()
        {
            return (from ManagementBaseObject o in Searcher.Get()
                select o as ManagementObject
                into device
                let driverInfo = GetDriverInfo(device)
                where device != null
                select new Device
                {
                    Name = device["Name"]?.ToString() ?? "",
                    ClassGuid = device["ClassGuid"]?.ToString() ?? "",
                    HardwareId = device["HardwareID"] == null ? "" : string.Join("\n", (string[]) device["HardwareID"]),
                    Manufacturer = device["Manufacturer"]?.ToString() ?? "",
                    Description = driverInfo[0],
                    Provider = device["Caption"]?.ToString() ?? "",
                    SysPath = driverInfo[1],
                    DevicePath = device["DeviceID"]?.ToString() ?? "",
                    Status = device["Status"]?.ToString().Equals("OK") ?? false
                }).ToList();
        }

        /// <summary>
        /// Gets information about device
        /// </summary>
        /// <param name="device"></param>
        /// <returns>Array of strings with</returns>
        private string[] GetDriverInfo(ManagementObject device)
        {
            var driverInfo = new string[2];
            foreach (var driverParameter in device.GetRelated("Win32_SystemDriver"))
            {
                driverInfo[0] += driverParameter["Description"] == null ? "" : driverParameter["Description"] + "\n";
                driverInfo[1] += driverParameter["PathName"] == null ? "" : driverParameter["PathName"] + "\n";
            }
            return driverInfo;
        }
    }
}
