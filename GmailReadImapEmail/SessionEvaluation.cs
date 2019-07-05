using System;
using System.Data;

namespace GmailReadImapEmail
{
    class SessionEvaluation : IEmailDerivative
    {
        public string SessionTitle;
        public string FromEmail;
        public string Subject;
        public string Contents;
        public DateTime SendDateTime;

        public SessionEvaluation(string sessionTitle, string fromEmail, string subject, string contents, DateTime sendDateTime)
        {
            SessionTitle = sessionTitle;
            Contents = contents;
            FromEmail = fromEmail;
            Subject = subject;
            SendDateTime = sendDateTime;
        }

        public DataTable GetDataTableWithSchema()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("SessionTitle", typeof(string));
            dt.Columns.Add("FromEmail", typeof(string));
            dt.Columns.Add("Subject", typeof(string));
            dt.Columns.Add("Contents", typeof(string));
            dt.Columns.Add("SendDateTime", typeof(DateTime));
            return dt;
        }

        public string GetProcedureName()
        {
            return "spDBA_InsertSessionEvaluations";
        }

        public string GetProcedureParameter()
        {
            return "@SessionEvaluations";
        }

        public string GetTypeName()
        {
            return "dbo.SessionEvaluationType";
        }

        public object[] ToObjectArray()
        {
            return new object[] { SessionTitle, FromEmail, Subject, Contents, SendDateTime };
        }
    }
}
