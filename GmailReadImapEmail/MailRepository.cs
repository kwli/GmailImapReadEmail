using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using System;
using System.Collections.Generic;

namespace GmailReadImapEmail
{
    public class MailRepository
    {
        private readonly string mailServer, login, password;
        private readonly int port;
        private readonly bool ssl;

        private Dictionary<JobType, SearchQuery> _searchQueries = new Dictionary<JobType, SearchQuery>()
        {
            { JobType.REMINDER, SearchQuery.FromContains("info@developerdays.pl").Or(SearchQuery.FromContains("info@join-conference.com"))
                .And(SearchQuery.SubjectContains("Payment deadline approaching").Or(SearchQuery.SubjectContains("Zbliżający się termin płatności")))
                .And(SearchQuery.BodyContains("you registered for").Or(SearchQuery.BodyContains("przeprowadziłeś rejestrację na"))) },
            { JobType.MAIL_DELIVERY_SUBSYSTEM, SearchQuery.SubjectContains("Delivery Status Notification") },
            { JobType.SESSION_EVALUATION, SearchQuery.SubjectContains("Evaluation of your session") },
            { JobType.TICKET_FUCKUP, SearchQuery.DeliveredAfter(new DateTime(2019, 5, 23)).And(SearchQuery.BodyContains("You can find the ticket for Cloud DeveloperDays 2019 attached.").Or(SearchQuery.BodyContains("W załączeniu znajdziesz bilety na Cloud DeveloperDays 2019."))) }
        };

        public MailRepository(string mailServer, int port, bool ssl, string login, string password)
        {
            this.mailServer = mailServer;
            this.port = port;
            this.ssl = ssl;
            this.login = login;
            this.password = password;
        }

        public MailRepository() : this(ConfigurationProvider.ImapServer, ConfigurationProvider.ImapPort, ConfigurationProvider.ImapSsl, ConfigurationProvider.InboxAddress, ConfigurationProvider.InboxPassword)
        {
        }

        public bool ProcessEmails(JobType jobType)
        {
            List<IEmailDerivative> derivativesList = new List<IEmailDerivative>();

            using (var client = new ImapClient())
            {
                client.Connect(mailServer, port, ssl);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(login, password);
                client.Inbox.Open(FolderAccess.ReadOnly);

                int d = 0;
                foreach (UniqueId uid in client.Inbox.Search(_searchQueries[jobType]))
                {
                    MimeMessage mm = client.Inbox.GetMessage(uid);
                    //if (mm.HtmlBody.IndexOf("localhost") == -1) // remove this line if necessary
                    //{
                        if (ConfigurationProvider.TestMode)
                        {
                            Logger.AddMessageToLog(mm);
                        }
                    try
                    {
                        IEmailDerivative emailDerivative = ProduceEmailDerivative(mm, jobType);
                        if (emailDerivative != null)
                        {
                            derivativesList.Add(emailDerivative);
                            Console.WriteLine("{0}", ++d);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                    //}
                }
                client.Disconnect(true);
            }
            
            return DbProvider.Synchronize(derivativesList);
        }

        private IEmailDerivative ProduceEmailDerivative(MimeMessage message, JobType jobType)
        {
            switch (jobType)
            {
                case JobType.REMINDER:
                    string docNumber = "";
                    IEnumerator<MimeEntity> en = message.Attachments.GetEnumerator();
                    en.MoveNext();
                    if (en.Current != null)
                    {
                        docNumber = en.Current.ContentType.Name;
                    }
                    int proIndex = docNumber.IndexOf("PRO_");
                    return proIndex != -1 ? new Reminder(docNumber.Substring(proIndex, docNumber.Length - proIndex - 4), message.To[0].ToString(), message.Date.DateTime) : null;
                case JobType.MAIL_DELIVERY_SUBSYSTEM:
                    return message.From.Count == 1 ? new MailDeliverySubsystem(message.Date.DateTime, ((MailboxAddress)message.From[0]).Address, message.Subject, message.TextBody) : null;
                case JobType.SESSION_EVALUATION:
                    if (!string.IsNullOrEmpty(message.HtmlBody) && message.From.Count == 1 && message.From[0] != null)
                    {
                        string fromEmail = ((MailboxAddress)message.From[0]).Address;
                        if (message.HtmlBody.IndexOf("localhost") == -1 && fromEmail == "info@developerdays.pl" || fromEmail == "info@join-conference.com")
                        {
                            int startPosition = message.HtmlBody.IndexOf("<h2>") + 4;
                            return new SessionEvaluation(message.HtmlBody.Substring(startPosition, message.HtmlBody.IndexOf("</h2>") - startPosition).Trim(), fromEmail, message.Subject, message.HtmlBody, message.Date.DateTime);
                        }
                    }
                    return null;
                //case JobType.TICKET_FUCKUP:

                //    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}