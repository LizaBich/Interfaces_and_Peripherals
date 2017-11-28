using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Getting_USB_Devices
{
    /// <inheritdoc cref="Form"/>
    /// <summary>
    /// Class which describes the user interface
    /// </summary>
    public partial class Form1 : Form
    {
        private const int WmDevicechange = 0X219;

        /// <summary>
        /// List which contains usb-devices
        /// </summary>
        private List<UsbDevice> _devices;

        /// <summary>
        /// Represent devices on GUI
        /// </summary>
        private readonly DataTable _table = new DataTable();
        
        /// <inheritdoc cref="Form"/>
        /// <summary>
        /// Constructor
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WmDevicechange)
            {
                UpdateGrid();
            }
        }

        /// <summary>
        /// Handler of Loaded event
        /// </summary>
        /// <param name="sender">Who generates event</param>
        /// <param name="e">Event arguments</param>
        private void Form1_Load(object sender, EventArgs e)
        {
            _devices = new List<UsbDevice>();
            _table.Columns.Add("Name", typeof(string));
            _table.Columns.Add("Total free space", typeof(string));
            _table.Columns.Add("Occupied space", typeof(string));
            _table.Columns.Add("Total size", typeof(string));
            UpdateGrid();
            OutputGrid.DataSource = _table;
            Eject.Enabled = false;
            Timer.Enabled = true;
        }

        /// <summary>
        /// Method which updates data 
        /// </summary>
        private void UpdateGrid()
        {
            int currentPosition = 0;
            if (OutputGrid.CurrentRow != null)
            {
                currentPosition = OutputGrid.CurrentRow.Index;
            }
            _table.Clear();
            _devices = DeviceSearcher.GetDevices();
            foreach(UsbDevice device in _devices)
            {
                _table.Rows.Add(device.Name, device.TotalFreeSpace, device.OccupiedSpace, device.TotalSize);
            }
            if (OutputGrid.RowCount - 1 > currentPosition)
            {
                OutputGrid.Rows[currentPosition].Selected = true;
            }
        }

        /// <summary>
        /// Handler for SelectionChanged event
        /// </summary>
        /// <param name="sender">Who raised event</param>
        /// <param name="e">Event arguments</param>
        private void OutputGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (OutputGrid.CurrentRow == null) return;
            if (OutputGrid.CurrentRow.Index >= 0 && OutputGrid.CurrentRow.Index < _devices.Count)
            {
                Eject.Enabled = !_devices[OutputGrid.CurrentRow.Index].IsMtp;
            }
            else
            {
                Eject.Enabled = false;
            }
        }

        /// <summary>
        /// Handler for Click event
        /// </summary>
        /// <param name="sender">Who raised the event</param>
        /// <param name="e">Event arguments</param>
        private void Eject_Click(object sender, EventArgs e)
        {
            if (OutputGrid.CurrentRow != null)
            {
                Message.Text = _devices[OutputGrid.CurrentRow.Index].Eject();   
            }
        }

        /// <summary>
        /// Handler for the Tick event
        /// </summary>
        /// <param name="sender">Who raised the event</param>
        /// <param name="e">Event arguments</param>
        private void Timer_Tick(object sender, EventArgs e) => UpdateGrid();
    }
}
