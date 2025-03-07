using BPHN.ModelLayer;
using BPHN.ModelLayer.Others;
using Google.Protobuf.WellKnownTypes;
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
            for (int i = 0; i < where.Count; i++)
            {
                var item = where[i];
                query.Append($"{item.Column} {item.Operator} @where{i} and ");
            }
            query.Append("1 = 1");
            return query.ToString();
        }

        public (Dictionary<string, object?> param, string filter) BuildWhere(List<WhereCondition> filters)
        {
            var data = filters.Select((v, i) => new
            {
                col = v.Column,
                oper = v.Operator,
                param = $"@p{i}",
                val = v.Value ?? null,
                param1 = v.Values?.Select((v1, i1) => $"@x{i1}") ?? null,
                val1 = v.Values ?? null
            }).Select(v2 => new
            {
                param = new { v2.param, v2.val },
                where = $"{v2.col} {v2.oper} {v2.param}",
                param1 = v2.param1 != null && v2.val1 != null ? v2.param1.Select((v3, i3) => new { param = v3, val = v2.val1[i3] ?? null }) : null,
                where1 = v2.param1 != null && v2.val1 != null ? $"{v2.col} {v2.oper} ({(string.Join(",", v2.param1))})" : "",
            });

            var where = data.Where(x => x.param1 == null).Select(x => x.where).ToList();
            var param = data.Where(x => x.param1 == null).Select(x => x.param).ToList();
            if (param.Count() < data.Count())
            {
                param.AddRange(data.Where(x => x.param1 != null).SelectMany(x => x.param1));
                where.AddRange(data.Where(x => x.param1 != null).Select(x => x.where1));
            }

            return (param.ToDictionary(x => x.param, x => x.val), string.Join(" and ", where));
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
