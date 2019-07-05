using System;
using System.Data;

namespace GmailReadImapEmail
{
    public class Reminder : IEmailDerivative
    {
        public string DocNumber;
        public string Email;
        public DateTime SendDate;

        public Reminder(string docNumber, string email, DateTime sendDate)
        {
            DocNumber = docNumber;
            Email = email;
            SendDate = sendDate;
        }

        public object[] ToObjectArray()
        {
            return new object[] { DocNumber, Email, SendDate };
        }

        public DataTable GetDataTableWithSchema()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("DocNumber", typeof(string));
            dt.Columns.Add("Email", typeof(string));
            dt.Columns.Add("SendDate", typeof(DateTime));
            return dt;
        }

        public string GetProcedureName()
        {
            return "spDBA_InsertOldReminders";
        }

        public string GetProcedureParameter()
        {
            return "@Reminders";
        }

        public string GetTypeName()
        {
            return "dbo.ReminderType";
        }

        public override string ToString()
        {
            return string.Concat(Email, " | ", SendDate, " | ", DocNumber);
        }
    }
}
