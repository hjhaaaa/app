using Model.EF;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common
{   
    /// <summary>
    /// info库连接串
    /// </summary>
    public class InfoDbContext : System.Data.Entity.DbContext
    {
        static string sqlstr = System.Configuration.ConfigurationManager.AppSettings["DbInfoConnectionString"];
        public InfoDbContext() : base(GetDb(),true)
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
        public DbSet<TbEfUser> tbEfUsers { get; set; }
       
    }
}
