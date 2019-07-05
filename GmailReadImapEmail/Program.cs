using System;

namespace GmailReadImapEmail
{
    class Program
    {
        static void Main(string[] args)
        {
            bool done = new MailRepository().ProcessEmails(ConfigurationProvider.JobType);
            Console.WriteLine("The end of execution");
            Console.ReadLine();
        }
    }
}
