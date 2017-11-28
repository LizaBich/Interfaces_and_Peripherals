using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab5_DeviceManager
{
    public partial class Form1 : Form
    {
        private readonly List<Device> _devices;

        private int _item;

        public Form1()
        {
            InitializeComponent();
            _devices = DeviceManager.GetDevices();
            foreach (var device in _devices)
            {
                ListOfDevices.Items.Add(device.Name);
            }
            TurnOn.Click += TurnOnClicked;
            TurnOff.Click += TurnOffClicked;
            ListOfDevices.Click += ItemSelected;
        }

        private void ShowInformation(Device device)
        {
            DeviceDescription.Text = "GUID: " + device.ClassGuid +
                                     "\r\n\r\nHardwareID: " + device.HardwareId +
                                     "\r\n\r\nManufacturer: " + device.Manufacturer +
                                     "\r\n\r\nProvider: " + device.Provider +
                                     "\r\n\r\nDriver description: " + device.Description +
                                     "\r\n\r\nDevice path: " + device.DevicePath +
                                     "\r\n\r\nSystem path: " + device.SysPath;
            TurnOn.Enabled = !device.Status;
            TurnOff.Enabled = device.Status;
        }

        private void ItemSelected(object sender, EventArgs e)
        {
            ShowInformation(_devices[ListOfDevices.SelectedItems[0].Index]);
            _item = ListOfDevices.SelectedItems[0].Index;
        }

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
                MessageBox.Show(exception.Message + ": " + exception.StackTrace);
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
