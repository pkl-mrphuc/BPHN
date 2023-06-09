using BPHN.ModelLayer.Others;
using System.Data;

namespace BPHN.DataLayer
{
    public interface IBaseRepository
    {
        IDbConnection ConnectDB(string connectionString);
        string GetConnectionString();
        string BuildWhereQuery(List<WhereCondition> where);
    }
}
