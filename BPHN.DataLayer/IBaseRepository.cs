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
    public interface IBaseRepository
    {
        IDbConnection ConnectDB(string connectionString);
        string GetConnectionString();
    }
}
