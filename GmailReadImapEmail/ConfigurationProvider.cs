using System;
using System.Configuration;

namespace GmailReadImapEmail
{
    public static class ConfigurationProvider
    {
        public static string ConnectionString => ConfigurationManager.ConnectionStrings["DATAMASTER"].ConnectionString;
        public static JobType JobType => (JobType)Enum.Parse(typeof(JobType), ConfigurationManager.AppSettings["JobType"]);
        public static string DbPrefix => ConfigurationManager.AppSettings["DbPrefix"];
        public static string ImapServer => ConfigurationManager.AppSettings["ImapServer"];
        public static int ImapPort => int.Parse(ConfigurationManager.AppSettings["ImapPort"]);
        public static bool ImapSsl => int.Parse(ConfigurationManager.AppSettings["ImapSsl"]) == 1;
        public static string InboxAddress => ConfigurationManager.AppSettings["InboxAddress"];
        public static string InboxPassword => ConfigurationManager.AppSettings["InboxPassword"];
        public static bool TestMode => int.Parse(ConfigurationManager.AppSettings["TestMode"]) == 1;
    }
}
