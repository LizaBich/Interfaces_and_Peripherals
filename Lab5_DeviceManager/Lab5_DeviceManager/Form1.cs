using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Lab5_DeviceManager
{
    /// <summary>
    /// Class that describes user GUI
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// List of devices
        /// </summary>
        private readonly List<Device> _devices;

        /// <summary>
        /// Device manager
        /// </summary>
        private readonly DeviceManager _manager = new DeviceManager();

        /// <summary>
        /// index of current item
        /// </summary>
        private int _item;

        /// <summary>
        /// Class constructor
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            _devices = _manager.GetDevices();
            foreach (var device in _devices)
            {
                ListOfDevices.Items.Add(device.Name);
            }
            TurnOn.Click += TurnOnClicked;
            TurnOff.Click += TurnOffClicked;
            ListOfDevices.Click += ItemSelected;
        }

        /// <summary>
        /// Method that shows information about selected device 
        /// </summary>
        /// <param name="device">Selected device</param>
        private void ShowInformation(Device device)
        {
            var parameters = new[]
            {
                "GUID: ",
                "HardwareID: ",
                "Manufacturer: ",
                "Provider: ",
                "Driver description: ",
                "Device path: ",
                "System path: "
            };

            var textForPrint = new StringBuilder();
            textForPrint.AppendLine(parameters[0] + device.ClassGuid).AppendLine(parameters[1] + device.HardwareId)
                .AppendLine(parameters[2] + device.Manufacturer).AppendLine(parameters[3] + device.Provider)
                .AppendLine(parameters[4] + device.Description).AppendLine(parameters[5] + device.DevicePath)
                .AppendLine(parameters[6] + device.SysPath);

            DeviceDescription.Text = textForPrint.ToString();
            TurnOn.Enabled = !device.Status;
            TurnOff.Enabled = device.Status;
        }

        /// <summary>
        /// Event handler for ItemSelected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ItemSelected(object sender, EventArgs e)
        {
            ShowInformation(_devices[ListOfDevices.SelectedItems[0].Index]);
            _item = ListOfDevices.SelectedItems[0].Index;
        }

        /// <summary>
        /// Event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TurnOnClicked(object sender, EventArgs e)
        {
            try
            {
                _devices[_item].ChangeConnection("Enable");
                _devices[_item].Status = !_devices[_item].Status;
                TurnOn.Enabled = !TurnOn.Enabled;
            }
            catch (IndexOutOfRangeException indexOutOfRangeException)
            {
                MessageBox.Show(indexOutOfRangeException.Message + ": " + indexOutOfRangeException.StackTrace);
            }
            catch (Exception exception)
            {
                MessageBox.Show($"{exception.Message}: {exception.StackTrace}");
            }
        }

        private void TurnOffClicked(object sender, EventArgs e)
        {
            try
            {
                _devices[_item].ChangeConnection("Disable");
                _devices[_item].Status = !_devices[_item].Status;
                TurnOff.Enabled = !TurnOff.Enabled;
            }
            catch (IndexOutOfRangeException indexOutOfRangeException)
            {
                MessageBox.Show(indexOutOfRangeException.Message + ": " + indexOutOfRangeException.StackTrace);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message + ": " + exception.StackTrace);
            }
        }
    }
}
