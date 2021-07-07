using App.Bll;

using common;
using Dapper;
using INFOModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace App.Api
{   
    
    public class APPInfoController : ApiController
    {
        /// <summary>
        /// 日记列表
        /// </summary>
        /// <param name="Mobile"></param>
        /// <returns></returns>
        [HttpGet]

        public List<TbUserNote> notelist()
        {
            var UserToken = TokenFilter.User.UserToken;
            var userinfo = UserBll.GetUserInfo(UserToken);
            using (var app = AppSqlCnn.GetInfoCnn()) {
                return app.Query<TbUserNote>("select id,title , Notes from  TbUserNote where UserId=@UserId and IsDelete=0",new { UserId = userinfo.Id }).ToList();
            }
        }
        /// <summary>
        /// 日记保存、删除,添加
        /// </summary>
        /// <param name="Mobile"></param>
        /// <param name="Id"></param>
        /// <param name="title"></param>
        /// <param name="note"></param>
        /// <param name="isadd">1 add 2up 3 del</param>
        /// <returns></returns>
        [HttpGet]

        public Webapiresult notesave(int Id,string title,string note,int isadd)
        {
            var UserToken = TokenFilter.User.UserToken;
            var userinfo = UserBll.GetUserInfo(UserToken);
            using (var app = AppSqlCnn.GetInfoCnn()) {
                if (isadd == 1) {
                    TbUserNote note1 = new TbUserNote {
                        CTime = DateTime.Now,
                        UserId = userinfo.Id,
                        IsDelete = 0,
                        Notes = note,
                        Title = title,
                        UTime = DateTime.Now
                    };
                    app.Insert(note1);

                } else if (isadd == 2) {
                    app.Execute("update  TbUserNote set title=@title  ,Notes=@note  where  id=@id",new { id = Id,title = title,note = note });
                } else if (isadd == 3) {
                    app.Execute("update   TbUserNote   set  IsDelete=1 where  id=@id",new { id = Id });
                }
                return new Webapiresult {
                    code = Webapiresult.webapicode.ok,
                    msg = "ok"
                };
            }
        }
       
    }
}
