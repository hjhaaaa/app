using common;
using Dapper;
using Model.EF;
using Model.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Bll
{
    /// <summary>
    /// 用户信息处理类
    /// </summary>
    public class UserBll
    {
        public static UserInfo GetUserInfo(string UserToken)
        {
            using (var app = new dbcontext()) {
                return app.MDapper.Query("select * from UserInfo where UserToken=@UserToken",new { UserToken = UserToken }
                    ).FirstOrDefault();               
            }
        }
        public static TbEfUser GetTbEfUser(int Id)
        {
            using (var app = new InfoDbContext()) {
                return app.tbEfUsers.Find(Id);
            }
                
        }
        public static int UpdateTbEfUser(TbEfUser efUser)
        {
            using (var app = new InfoDbContext()) {
                app.Entry(efUser).State = System.Data.Entity.EntityState.Modified;
                return app.SaveChanges();
            }
            
        }
        public static int DeleteTbEfUser(int Id)
        {
            using (var app = new InfoDbContext()) {
                app.tbEfUsers.Remove(GetTbEfUser(Id));
                return app.SaveChanges();
            }
        }
        public static int ADDTbEfUser(TbEfUser efUser)
        {
            using (var app = new InfoDbContext()) {
                app.tbEfUsers.Add(efUser);
                return app.SaveChanges();
            }
        }
    }
}
