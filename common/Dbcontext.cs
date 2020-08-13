using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;

namespace common
{
    public class dbcontext:System.Data.Entity.DbContext
    {
        static string sqlstr = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
        public dbcontext() : base(GetDb(), true)
        {

        }
        static DbConnection GetDb()
        {
            DbConnection db = new MySqlConnection(sqlstr);
            Dapper.SimpleCRUD.SetDialect(Dapper.SimpleCRUD.Dialect.MySQL);
           
            return db;
        }
        public DbConnection MDapper {
            get { return this.Database.Connection; }
        }
    }
}
