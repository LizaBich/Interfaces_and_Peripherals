using System;
using System.Windows.Forms;

namespace Lab3_Battery
{
    public partial class Form1 : Form
    {
        private readonly Battery _battery = new Battery();
        private readonly Timer _timer = new Timer();

        public Form1()
        {
            InitializeComponent();
            
            ConnectType.Text = _battery.GetConnectType();
            AvailablePower.Text = _battery.GetAvailPower();
            AvailableTime.Text = _battery.GetAvailTime();

            _timer.Interval = 1000;
            _timer.Tick += TimerEvent;
            _timer.Start();

            Closing += AppClose;
        }

        private void TimerEvent(object sender, EventArgs e)
        {
            ConnectType.Text = _battery.GetConnectType();
            AvailablePower.Text = _battery.GetAvailPower();
            AvailableTime.Text = _battery.GetAvailTime();
        }

        private void AppClose(object sender, EventArgs e)
        {
            _timer.Stop();

        }
    }
}
