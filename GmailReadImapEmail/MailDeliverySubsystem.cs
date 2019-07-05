using System;
using System.Data;

namespace GmailReadImapEmail
{
    public class MailDeliverySubsystem : IEmailDerivative
    {
        public DateTime Date;
        public string From;
        public string Title;
        public string Contents;

        public MailDeliverySubsystem(DateTime date, string from, string title, string contents)
        {
            Date = date;
            From = from;
            Title = title;
            Contents = contents;
        }

        public object[] ToObjectArray()
        {
            return new object[] { Date, From, Title, Contents };
        }

        public DataTable GetDataTableWithSchema()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Date", typeof(DateTime));
            dt.Columns.Add("From", typeof(string));
            dt.Columns.Add("Title", typeof(string));
            dt.Columns.Add("Contents", typeof(string));
            return dt;
        }

        public string GetProcedureName()
        {
            return "spDBA_InsertMailDeliverySubsystems";
        }

        public string GetProcedureParameter()
        {
            return "@MailDeliverySubsystems";
        }

        public string GetTypeName()
        {
            return "dbo.MailDeliverySubsystemType";
        }

        public override string ToString()
        {
            return string.Concat(Date.ToString(), " | ", From, " | ", Title, " | \n\n", Contents);
        }
    }
}
