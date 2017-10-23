using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Lab3_Battery
{
    /// <summary>
    /// Класс, позволяющий получить основную информацию о батарее.
    /// </summary>
    class Battery
    {
        /// <summary>
        /// Свойство, описывающее оставшийся заряд батареи.
        /// </summary>
        public string AvailPower { get; set; }

        /// <summary>
        /// Свойство, описывающее оставшееся время работы от батареи.
        /// </summary>
        public int AvailTime { get; set; }

        /// <summary>
        /// Свойсто, описывающее тип подключенного источника питания.
        /// </summary>
        public string ConnectType { get; set; }

        /// <summary>
        /// Время отключения монитора до вмешательства.
        /// </summary>
        private int ScreenTime { get; set; }

        /// <summary>
        /// Конструктор класса.
        /// </summary>
        public Battery()
        {
            GetTime();
        }

        /// <summary>
        /// Получает тип подключенного источника питания.
        /// </summary>
        /// <returns>Тип источника питания</returns>
        public string GetConnectType()
        {
            ConnectType = SystemInformation.PowerStatus.PowerLineStatus.ToString();
            switch (ConnectType)
            {
                case "Online": return "AC";
                case "Offline": return "Battery";
                default: return "Unknown";
            }
        }

        /// <summary>
        /// Получает оставшееся время работы от батареи.
        /// </summary>
        /// <returns>Оставшееся время работы</returns>
        public string GetAvailTime()
        {
            AvailTime = SystemInformation.PowerStatus.BatteryLifeRemaining;
            if (AvailTime != -1 && ConnectType == "Offline")
            {
                return new TimeSpan(0, AvailTime / 60, 0).ToString();
            }
            if (AvailTime == -1 && ConnectType == "Online")
            {
                return "заряжается.";
            }
            return "идёт подсчёт...";
        }

        /// <summary>
        /// Получает оставшийся заряд батареи в %.
        /// </summary>
        /// <returns></returns>
        public string GetAvailPower()
        {
            AvailPower = Convert.ToString(SystemInformation.PowerStatus.BatteryLifePercent * 100, CultureInfo.CurrentCulture) + "%";
            return AvailPower;
        }

        /// <summary>
        /// Меняет время отключения экрана
        /// </summary>
        public void DisableScreen(int newTime)
        {
            Process cmd = new Process
            {
                StartInfo =
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = "/c powercfg /x -monitor-timeout-dc " + newTime 
                }
            };

            cmd.Start();
        }

        /// <summary>
        /// Получение исходного времени отключения экрана.
        /// </summary>
        private void GetTime()
        {
            Process cmd = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = "/c powercfg /q"
                }
            };
            cmd.Start();

            var powerSchemes = cmd.StandardOutput.ReadToEnd();
            var someString = new Regex("VIDEOIDLE.*\\n.*\\n.*\\n.*\\n.*\\n.*\\n.*");
            var videoidle = someString.Match(powerSchemes).Value;
            ScreenTime = Convert.ToInt32(videoidle.Substring(videoidle.Length - 11).TrimEnd(), 16) / 60;
        }

        /// <summary>
        /// Восстановление времени отключения экрана.
        /// </summary>
        public void ReturnOldTimeValue() => DisableScreen(ScreenTime);
    }
}
