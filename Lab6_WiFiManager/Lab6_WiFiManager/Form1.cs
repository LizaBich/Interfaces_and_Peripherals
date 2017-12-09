using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Lab6_WiFiManager
{
    /// <summary>
    /// Represents the GUI
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Wifi-networks list
        /// </summary>
        private List<WiFiNetwork> _networks;

        /// <summary>
        /// The index of the current networt
        /// </summary>
        private int _index;

        /// <summary>
        /// Simple timer
        /// </summary>
        private readonly Timer _timer;

        private readonly WiFiManager _manager;

        /// <summary>
        /// Message which displayed when connection is existing
        /// </summary>
        private const string ConnectString = "Connected";

        /// <summary>
        /// Class constructor
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            _manager = new WiFiManager();
            UpdateData();
            ConnectToNet.Enabled = false;
            Password.Enabled = false;
            _timer = new Timer
            {
                Interval = 5000,
                Enabled = true
            };
            _timer.Tick += Timer_Tick;
            ConnectToNet.Click += ConnectToNet_Click;
            ListOfNetworks.Click += ItemSelected;
            _timer.Start();
        }

        /// <summary>
        /// Updates the list of the networks
        /// </summary>
        private void UpdateData()
        {
            ListOfNetworks.Clear();
            _networks = _manager.GetWiFiNetworks();
            foreach (var network in _networks)
            {
                ListOfNetworks.Items.Add(network.Name + " (" + network.Strength + ")");
            }
        }

        /// <summary>
        /// Displays information about a network
        /// </summary>
        /// <param name="network">The network</param>
        private void ShowInformation(WiFiNetwork network)
        {
            const string availableString = "Available";
            Description.Text = network.Description;
            if (network.IsConnected)
            {
                Status.Text = ConnectString;
                Password.Enabled = ConnectToNet.Enabled = false;
            }
            else
            {
                Status.Text = availableString;
                Password.Enabled = network.IsSecure;
                ConnectToNet.Enabled = true;
            }
        }

        /// <summary>
        /// Handler for the ItemSelected event
        /// </summary>
        /// <param name="sender">Who raised the event</param>
        /// <param name="args">Event arguments</param>
        private void ItemSelected(object sender, EventArgs args)
        {
            _index = ListOfNetworks.SelectedItems[0].Index;
            ShowInformation(_networks[_index]);
        }

        /// <summary>
        /// Handler for the Tick event
        /// </summary>
        /// <param name="sender">Who raised the event</param>
        /// <param name="args">Event arguments</param>
        private void Timer_Tick(object sender, EventArgs args)
        {
            if (ListOfNetworks.SelectedItems.Count == 0)
            {
                UpdateData();
            }
        }

        /// <summary>
        /// Handler for the Click event
        /// </summary>
        /// <param name="sender">Who raised the event</param>
        /// <param name="args">Event arguments</param>
        private void ConnectToNet_Click(object sender, EventArgs args)
        {
            const string error = "Error";
            if (Password.Text.Length <= 0) return;
            if (_networks[_index].Connect(Password.Text ?? string.Empty))
            {
                Status.Text = ConnectString;
                Password.Enabled = false;
                ConnectToNet.Enabled = false;
            }
            else Status.Text = error;
        }
    }
}
