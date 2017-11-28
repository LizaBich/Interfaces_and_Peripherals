using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Lab6_WiFiManager
{
    public partial class Form1 : Form
    {
        private List<WiFiNetwork> _networks;
        private int _index;
        private readonly Timer _timer;
        private const string ConnectString = "Connected";

        public Form1()
        {
            InitializeComponent();
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

        private void UpdateData()
        {
            ListOfNetworks.Clear();
            _networks = WiFiManager.GetWiFiNetworks();
            foreach (var network in _networks)
            {
                ListOfNetworks.Items.Add(network.Name + " (" + network.Strength + ")");
            }
        }

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

        private void ItemSelected(object sender, EventArgs args)
        {
            _index = ListOfNetworks.SelectedItems[0].Index;
            ShowInformation(_networks[_index]);
        }

        private void Timer_Tick(object sender, EventArgs args)
        {
            if (ListOfNetworks.SelectedItems.Count == 0)
            {
                UpdateData();
            }
        }

        private void ConnectToNet_Click(object sender, EventArgs args)
        {
            const string error = "Error";
            if (Password.Text.Length <= 0) return;
            if (_networks[_index].Connect(Password.Text))
            {
                Status.Text = ConnectString;
                Password.Enabled = false;
                ConnectToNet.Enabled = false;
            }
            else Status.Text = error;
        }
    }
}
