using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;

namespace common
{
    public class AppSqlCnn
    {
        static string sqlstr = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
        static string sqlInfostr = System.Configuration.ConfigurationManager.AppSettings["DbInfoConnectionString"];
        public static DbConnection GetAppCnn()
        {
            DbConnection db = new MySqlConnection(sqlstr);
            Dapper.SimpleCRUD.SetDialect(Dapper.SimpleCRUD.Dialect.MySQL);

            return db;
        }
        public static DbConnection GetInfoCnn()
        {
            DbConnection db = new MySqlConnection(sqlInfostr);
            Dapper.SimpleCRUD.SetDialect(Dapper.SimpleCRUD.Dialect.MySQL);

            return db;
        }
       
    }
}
