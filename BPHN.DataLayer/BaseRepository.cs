using BPHN.ModelLayer;
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
