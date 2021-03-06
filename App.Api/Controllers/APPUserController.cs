﻿using App.Api;
using App.Bll;
using App.Cache.LoginCache;
using App.DTO;

using common;
using common.Tool;
using Dapper;

using Model;
using Model.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace App.Api
{
    [TokenFilter(IsCheckLogin = false)]
    public class APPUserController : ApiController
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 版本比较
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        [HttpGet]
       
        public List<versioninfo> cop(int version)
        {
          //  string path = AppDomain.CurrentDomain.BaseDirectory + "version.txt";
            using(var app= AppSqlCnn.GetAppCnn())
            {
                return app.Query<versioninfo>("select * from  versioninfo where vindex>@index and isdebug=0", new { index = version }).ToList();
            }       
        }
        /// <summary>
        /// 获取客户端logo
        /// </summary>
        /// <returns></returns>
        [HttpGet]
       
       
        public IWebApiResult getlogo()
        {
            
            using (var app = AppSqlCnn.GetAppCnn())
            {
                var l= app.Query<string>("select logo from  applogo  where  status=0").FirstOrDefault();
                return WebApiResult<string>.Success(l);
            }
            
        }
        /// <summary>
        /// 下载更新文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        [HttpGet]
      
        public async Task<HttpResponseMessage> download(string file ,string version)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin\\down\\"+version+"\\"+file);
                if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                {

                    string filename = Path.GetFileName(path);
                    var stream = new FileStream(path, FileMode.Open);
                    HttpResponseMessage resp = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StreamContent(stream)
                    };
                    resp.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                    {
                        FileName = filename
                    };
                    resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                    resp.Content.Headers.ContentLength = stream.Length;

                    return await Task.FromResult(resp);
                }
            }
            catch (Exception ex)
            {
            }
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [HttpGet]
        //[Route("api/app/user/login")]
        public WebapiresultLogin login(string mobile,string pwd)
        {
           
            using (var app = AppSqlCnn.GetAppCnn())
            {
              var count=  app.Query<UserInfo>("select * from userinfo where mobile=@mobile and PwdMd5=@PwdMd5 and Status=0",new { mobile= mobile,PwdMd5 = pwd.Md5Deal()}).FirstOrDefault();
                if(count==null)
                {
                    return new WebapiresultLogin { code = Webapiresult. webapicode.fail, msg = "账号密码错误" };
                }
                else
                {
                    var Token = Guid.NewGuid().ToString("N");
                    LoginCache loginCache = new LoginCache(count.UserToken);
                    loginCache.LoginToken = Token;
                    loginCache.Set();
                    TokenFilter.LoginUser user = new TokenFilter.LoginUser {
                        Mobile = mobile,Name = count.UserName,Token = Token,UserToken = count.UserToken
                    };
                    TokenFilter.SetCookie(user,DateTime.Now.AddDays(7));
                    return new WebapiresultLogin { code = Webapiresult.webapicode.ok, msg = "ok",Data=mobile,Data2= count.UserName,Usertoken = count.UserToken,Token= Token };
                }
            }              
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [HttpGet]
       
        public Webapiresult reg(string mobile, string pwd)
        {
            using (var app = AppSqlCnn.GetAppCnn())
            {
                if (!mobile.IsMobile()) {
                    return new Webapiresult { code = Webapiresult.webapicode.fail,msg = "手机号格式不正确" };
                }
                if (!pwd.IsPassword()) {
                    return new Webapiresult { code = Webapiresult.webapicode.fail,msg = "密码格式不正确" };
                }
                int count = app.Query<int>("select count(1) from userinfo where mobile=@mobile ", new { mobile = mobile,PwdMd5 = pwd.Md5Deal() }).FirstOrDefault();
                if (count > 0)
                {
                    return new Webapiresult { code = Webapiresult.webapicode.fail, msg = "此用户已注册" };
                }
                else
                {
                    UserInfo usersinfo = new UserInfo {
                        CTime = DateTime.Now,
                        Mobile = mobile,
                        Pwd = pwd,
                        UserName = mobile,
                        Status = 0,
                        UserToken = Guid.NewGuid().ToString("N"),
                        PwdMd5 = pwd.Md5Deal()
                    };
                    app.Insert(usersinfo);
                    return new Webapiresult { code = Webapiresult.webapicode.ok, msg = "ok" };
                }
            }
        }
        /// <summary>
        /// 设置用户昵称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet]
        [TokenFilter(IsCheckLogin=true)]
      
        public Webapiresult setusername(string name)
        {
            var UserToken=TokenFilter.User.UserToken;
            using (var app = AppSqlCnn.GetAppCnn()) {
                var user = app.Query<UserInfo>("select * from userinfo where UserToken=@UserToken ",new { UserToken = UserToken,}).FirstOrDefault();
                if (user != null) {
                    user.UserName = name;
                    app.Update(user);
                }
            }
                return new Webapiresult { code = Webapiresult.webapicode.ok,msg = "ok" };
        }
        
       
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
      
        public async Task< Webapiresult> UpImag()
        {
            var context = HttpContext.Current.Request;
            if (context.Files.Count != 1) {
                throw new Exception("一次只能上传一个图");
            }
           var stream= context.Files[0].InputStream;
            var path = HttpRuntime.AppDomainAppPath.ToString() + "//";
            var path2="image//" +DateTime.Now.Ticks+Guid.NewGuid().ToString("N").Substring(0,6)+".jpg";
            using (FileStream fs = new FileStream(path +path2,FileMode.Create)) {
                await stream.CopyToAsync(fs);
            }
            return Webapiresult.Ok();
        }
    }
}
