using App.Cache.LoginCache;
using AppUser.App_Start;
using common;
using common.Tool;
using Dapper;
using Model;
using Model.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;


namespace verupserver.Controllers
{
    public class APPUserController : ApiController
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
      
        [HttpGet]
        [Route("api/compareveison/cop")]
        public List<versioninfo> cop(int version)
        {
          //  string path = AppDomain.CurrentDomain.BaseDirectory + "version.txt";
            using(var app=new dbcontext())
            {
                return app.MDapper.Query<versioninfo>("select * from  versioninfo where vindex>@index and isdebug=0", new { index = version }).ToList();
            }       
        }
        /// <summary>
        /// 获取客户端logo
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/download/logo")]
        public string getlogo()
        {
            //string[] vss = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "bin\\logo.txt");
            //foreach(var v in vss)
            //{
            //    if (!v.StartsWith("#")) { return v; }
            //}
            using (var app = new dbcontext())
            {
                return app.MDapper.Query<string>("select logo from  logo  where  status=1").FirstOrDefault();
            }
            return "";
        }
        [HttpGet]
        [Route("download/file")]
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
        [Route("api/app/user/login")]
        public WebapiresultLogin login(string mobile,string pwd)
        {
           
            using (var app =new dbcontext())
            {
              var count=  app.MDapper.Query<UserInfo>("select * from userinfo where mobile=@mobile and PwdMd5=@PwdMd5 and Status=0",new { mobile= mobile,PwdMd5 = pwd.Md5Deal()}).FirstOrDefault();
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
        [Route("api/app/user/reg")]
        public Webapiresult reg(string mobile, string pwd)
        {
            using (var app = new dbcontext())
            {
                if (!mobile.IsMobile()) {
                    return new Webapiresult { code = Webapiresult.webapicode.fail,msg = "手机号格式不正确" };
                }
                if (!pwd.IsPassword()) {
                    return new Webapiresult { code = Webapiresult.webapicode.fail,msg = "密码格式不正确" };
                }
                int count = app.MDapper.Query<int>("select count(1) from userinfo where mobile=@mobile ", new { mobile = mobile,PwdMd5 = pwd.Md5Deal() }).FirstOrDefault();
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
                    app.MDapper.Insert(usersinfo);
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
        [Route("api/app/user/setusername")]
        public Webapiresult setusername(string name)
        {
            var UserToken=TokenFilter.User.UserToken;
            using (var app = new dbcontext()) {
                var user = app.MDapper.Query<UserInfo>("select * from userinfo where UserToken=@UserToken ",new { UserToken = UserToken,}).FirstOrDefault();
                if (user != null) {
                    user.UserName = name;
                    app.MDapper.Update(user);
                }
            }
                return new Webapiresult { code = Webapiresult.webapicode.ok,msg = "ok" };
        }
        public class Note
        {
            public string Title {
                get;
                set;
            }
            public string Notes { get; set; }
            public int Id { get; set; }
        }
        /// <summary>
        /// 日记列表
        /// </summary>
        /// <param name="Mobile"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/note/list")]
        public List<Note> notelist(string Mobile)
        {
            using (var app = new dbcontext())
            {
              return  app.MDapper.Query<Note>("select id,title ,note as Notes from  note where mobile=@mobile and del=0",new { mobile =Mobile}).ToList();
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
        [Route("api/note/notesave")]
        public Webapiresult notesave(string Mobile,int Id,string title,string note,int isadd)
        {
            using (var app = new dbcontext())
            {  
                if(isadd==1)
                {
                    //NoteModel note1 = new NoteModel
                    //{
                    //    Del = 0,
                    //    Mobile = Mobile,
                    //    Note = note,
                    //    Title = title
                    //};
                  //  app.MDapper.Insert(note1);
                                
                }
                else if(isadd==2)
                {
                    app.MDapper.Execute("update  note set title=@title  ,note=@note  where mobile=@mobile and id=@id", new { mobile = Mobile, id = Id, title = title, note = note });
                }
                else if(isadd==3)
                {
                    app.MDapper.Execute("delete  from note   where mobile=@mobile and id=@id", new { mobile = Mobile, id = Id});                    
                }
                return new Webapiresult
                {
                    code = Webapiresult.webapicode.ok,
                    msg = "ok"
                };
            }
        }

    }
}
