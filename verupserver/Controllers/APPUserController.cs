using App.Bll;
using App.Cache.LoginCache;
using AppUser.App_Start;
using common;
using common.Tool;
using Dapper;
using INFOModel;
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
using System.Web;
using System.Web.Http;


namespace verupserver.Controllers
{
    public class APPUserController : ApiController
    {
        NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 版本比较
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
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
        [Route("api/app/logo")]
        public string getlogo()
        {
            
            using (var app = new dbcontext())
            {
                return app.MDapper.Query<string>("select logo from  applogo  where  status=0").FirstOrDefault();
            }
            
        }
        /// <summary>
        /// 下载更新文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="version"></param>
        /// <returns></returns>
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
        [Route("api/app/user/login")][TokenFilter(IsCheckLogin =false)]
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
        [Route("api/app/user/reg")]
        [TokenFilter(IsCheckLogin = false)]
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
        
        /// <summary>
        /// 日记列表
        /// </summary>
        /// <param name="Mobile"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/info/note/list")]
        public List<TbUserNote> notelist()
        {
            var UserToken = TokenFilter.User.UserToken;
            var userinfo = UserBll.GetUserInfo(UserToken);
            using (var app = new InfoDbContext())
            {
              return  app.MDapper.Query<TbUserNote>("select id,title , Notes from  TbUserNote where UserId=@UserId and IsDelete=0",new { UserId = userinfo.Id }).ToList();
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
        [Route("api/info/note/noteset")]
        public Webapiresult notesave(int Id,string title,string note,int isadd)
        {
            var UserToken = TokenFilter.User.UserToken;
           var userinfo= UserBll.GetUserInfo(UserToken);
            using (var app = new InfoDbContext())
            {  
                if(isadd==1)
                {
                    TbUserNote note1 = new TbUserNote {
                        CTime=DateTime.Now,
                         UserId= userinfo.Id,
                          IsDelete=0,
                           Notes=note,
                            Title=title, 
                             UTime=DateTime.Now
                    };
                    app.MDapper.Insert(note1);

                }
                else if(isadd==2)
                {
                    app.MDapper.Execute("update  TbUserNote set title=@title  ,Notes=@note  where  id=@id", new {  id = Id, title = title, note = note });
                }
                else if(isadd==3)
                {
                    app.MDapper.Execute("update   TbUserNote   set  IsDelete=1 where  id=@id", new {  id = Id});                    
                }
                return new Webapiresult
                {
                    code = Webapiresult.webapicode.ok,
                    msg = "ok"
                };
            }
        }
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/app/user/imgs/upimg")]
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
