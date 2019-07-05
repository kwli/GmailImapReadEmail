using MimeKit;
using System;
using System.IO;
using System.Text;

namespace GmailReadImapEmail
{
    public static class Logger
    {
        public static void AddMessageToLog(MimeMessage mimeMessage)
        {
            try
            {
                string dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log");
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{DateTime.Now:yyyyMMdd}.log");
                StringBuilder sb = new StringBuilder();
                //sb.AppendLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                //sb.AppendLine($"{mimeMessage.Date.DateTime:yyyy-MM-dd HH:mm:ss}");
                sb.AppendLine(mimeMessage.To.ToString());
                //sb.AppendLine(mimeMessage.Subject);
                //sb.AppendLine(mimeMessage.HtmlBody);
                //sb.AppendLine("--------------------------------------------------------------------------------------");
                File.AppendAllText(file, sb.ToString());
            }
            catch (Exception)
            {
                // something was wrong
            }
        }
    }
}
