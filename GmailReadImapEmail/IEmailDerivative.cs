using System.Data;

namespace GmailReadImapEmail
{
    public interface IEmailDerivative
    {
        string GetProcedureName();
        string GetProcedureParameter();
        string GetTypeName();
        object[] ToObjectArray();
        DataTable GetDataTableWithSchema();
    }
}
