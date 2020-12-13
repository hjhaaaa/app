using common;
using Model.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace APPBll
{   
    /// <summary>
    /// 用户信息处理类
    /// </summary>
    public class UserBll
    {
        public UserInfo GetUserInfo(string UserToken)
        {
            using (var app = new dbcontext()) {
                app.MDapper.Query()
            }
        }
    }
}
