using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

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
    }
}
