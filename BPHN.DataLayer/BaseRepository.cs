using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text;

namespace BPHN.DataLayer
{
    public class BaseRepository : IBaseRepository
    {
        private AppSettings _appSettings;
        public BaseRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public string BuildWhereQuery(List<WhereCondition> where)
        {
            var query = new StringBuilder();
            for(int i = 0; i < where.Count; i ++)
            {
                var item = where[i];
                query.Append($"{item.Column} {item.Operator} @where{i} and ");
            }
            query.Append("1 = 1");
            return query.ToString();
        }

        public IDbConnection ConnectDB(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }

        public string GetConnectionString()
        {
            return _appSettings.ConnectionString;
        }
    }
}
