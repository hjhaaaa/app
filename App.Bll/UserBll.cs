using common;
using Dapper;
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
    }
}
