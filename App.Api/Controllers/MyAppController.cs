using App.Api;
using App.Bll;
using App.DTO;
using common;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApp.Controllers
{
    [TokenFilter(IsCheckLogin = false)]
    public class MyAppController : ApiController
    {   
        [HttpGet]
        public string GetOne() 
        {
            return "one"; 
        }
        /// <summary>
        /// 获取客户端logo
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IWebApiResult logo()
        {
            using (var app = AppSqlCnn.GetAppCnn()) {
                var l = app.Query<string>("select logo from  applogo  where  status=0").FirstOrDefault();
                return WebApiResult<string>.Success(l);
            }
        }
        [HttpGet]
        
        public IWebApiResult EfTest()
        {
            UserBll.ADDTbEfUser(new Model.EF.TbEfUser { Name = "hjh",Address = "123" });
            return WebApiResult.Success();
        }
    }
}
