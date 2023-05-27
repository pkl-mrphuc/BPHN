using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                string next = i == where.Count - 1 ? "" : "and";
                query.Append($"{item.Column} {item.Operator} @where{i} {next} ");
            }
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
