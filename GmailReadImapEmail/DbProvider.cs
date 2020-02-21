using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace GmailReadImapEmail
{
    public static class DbProvider
    {
        public static bool Synchronize(List<IEmailDerivative> derivatives)
        {
            if (derivatives.Count < 1)
            {
                throw new Exception();
            }
            IEmailDerivative firstElement = derivatives[0];
            DataTable dt = firstElement.GetDataTableWithSchema();
            foreach (IEmailDerivative ied in derivatives)
            {
                dt.Rows.Add(ied.ToObjectArray());
            }

            Db.ExecuteNonQueryProcedure(firstElement.GetProcedureName(), new[]
            {
                new SqlParameter(firstElement.GetProcedureParameter(), SqlDbType.Structured) {TypeName = firstElement.GetTypeName(), Value = dt}
            });

            //using (StreamWriter sw = new StreamWriter(new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dupa.txt"), FileMode.CreateNew)))
            //{
            //    foreach (IEmailDerivative ied in derivatives)
            //    {
            //        MailDeliverySubsystem = ((MailDeliverySubsystem) ied).T
            //        sw.WriteLine(ied.ToObjectArray)

            //    }
            //}

            return true;
        }

        public static bool ProduceUpdatesList(List<IEmailDerivative> derivatives)
        {
            string updatesQuery = "";
            string updatePattern = "UPDATE Users SET UnsubscriptionDate = GETDATE(), UnsubscriptionActive = 0 WHERE UnsubscriptionDate IS NULL AND Email = '{0}'";
            string[] replaceArray = new string[] { "(", ")", "," };
            foreach (IEmailDerivative ied in derivatives)
            {
                string mailContent = ied.ToObjectArray()[3].ToString();
                int atPosition = mailContent.IndexOf("@");
                int firstSpacePosition = mailContent.Substring(0, atPosition).LastIndexOf(" ") + 1;
                int offset = 0;
                string finishTag = " ";
                if (mailContent.IndexOf("groups:") != -1)
                {
                    offset = 11;
                    finishTag = "<";
                }
                string email = mailContent.Substring(firstSpacePosition + offset, atPosition - firstSpacePosition - offset + mailContent.Substring(atPosition).IndexOf(finishTag));
                foreach (string stringToReplace in replaceArray)
                {
                    email = email.Replace(stringToReplace, "");
                }
                updatesQuery = string.Concat(updatesQuery, updatePattern.Replace("{0}", email), "\n");
            }
            using (StreamWriter outputFile = new StreamWriter(new FileStream(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dupa.txt"), FileMode.Create)))
            {
                outputFile.WriteLine(updatesQuery);
            }
            return true;
        }
    }
}
