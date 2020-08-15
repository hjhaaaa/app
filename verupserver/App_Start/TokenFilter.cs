using App.Cache.LoginCache;
using common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace AppUser.App_Start
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method,AllowMultiple = false)]
    public class TokenFilter : FilterAttribute, IAuthorizationFilter
    { /// <summary>
      /// 方法是否需要 登陆校验
      /// </summary>
        public bool IsCheckLogin { get; set; }
        private static string app_usertoken = "app_usertoken";
        private static string app_token = "app_token";
        private static string app_mobile = "app_mobile";
        private static string app_name = "app_name";
        private static string CookieDomain = "119.45.204.247";
        private static string CookieUser = "CookieUser";
       
        public static void RemoveLoginCookie()
        {
            {
                var authCookie = new HttpCookie(app_usertoken,"");
                authCookie.Domain = CookieDomain;
                //让所有的请求都带上前缀 节约流量
                //authCookie.Path = CookiePath;
                //authCookie.HttpOnly = CookieHttpOnly;
                authCookie.Expires = new DateTime(2000,1,1);
                HttpContext.Current.Response.Cookies.Add(authCookie);
            }

            {
                var authCookie = new HttpCookie(app_token,"");
                authCookie.Domain = CookieDomain;
                //让所有的请求都带上前缀 节约流量
                //authCookie.Path = CookiePath;
                //authCookie.HttpOnly = CookieHttpOnly;
                authCookie.Expires = new DateTime(2000,1,1);
                HttpContext.Current.Response.Cookies.Add(authCookie);
            }
            {
                var authCookie = new HttpCookie(app_mobile,"");
                authCookie.Domain = CookieDomain;
                //让所有的请求都带上前缀 节约流量
                //authCookie.Path = CookiePath;
                //authCookie.HttpOnly = CookieHttpOnly;
                authCookie.Expires = new DateTime(2000,1,1);
                HttpContext.Current.Response.Cookies.Add(authCookie);
            }
            {
                var authCookie = new HttpCookie(app_name,"");
                authCookie.Domain = CookieDomain;
                //让所有的请求都带上前缀 节约流量
                //authCookie.Path = CookiePath;
                //authCookie.HttpOnly = CookieHttpOnly;
                authCookie.Expires = new DateTime(2000,1,1);
                HttpContext.Current.Response.Cookies.Add(authCookie);
            }
        }
        public Task<HttpResponseMessage> ExecuteAuthorizationFilterAsync(HttpActionContext actionContext,CancellationToken cancellationToken,Func<Task<HttpResponseMessage>> continuation)
        {
            if (!IsCheckLogin) {
                //无需登陆校验
                return continuation();
            }
            var app_usertoken = "";
            var app_token = "";
            try {
                app_usertoken = actionContext.Request.Headers.GetCookies()[0].Cookies.First(x => x.Name == app_usertoken).Value;
                app_token = actionContext.Request.Headers.GetCookies()[0].Cookies.First(x => x.Name == app_token).Value;
            } catch {
                TokenFilter.RemoveLoginCookie();
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new ObjectContent<Webapiresult>(new Webapiresult {
                        code = Webapiresult.webapicode.tokenfail,
                        msg = $"token失效，请重新登陆",
                    },GlobalConfiguration.Configuration.Formatters.JsonFormatter)
                });
            }
            LoginCache loginCache = new LoginCache(app_usertoken);
            var chetoken = loginCache.Get()?.LoginToken ?? "";
            if (chetoken != app_token) {
                TokenFilter.RemoveLoginCookie();
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new ObjectContent<Webapiresult>(new Webapiresult {
                        code = Webapiresult.webapicode.tokenfail,
                        msg = $"token失效，请重新登陆",
                    },GlobalConfiguration.Configuration.Formatters.JsonFormatter)
                });
            }
            LoginUser user = new LoginUser {
                Token = app_token,
                UserToken = app_usertoken
            };
            HttpContext.Current.Items[CookieUser] = user;
            return continuation();
        }
        public static LoginUser User{
            get {
                var loginUser = HttpContext.Current.Items[CookieUser] as LoginUser;
                return loginUser;
            }
        }

        public class LoginUser
        {
            /// <summary>
            /// 用户Id
            /// </summary>
            public string UserToken { get; set; }


            /// <summary>
            /// Token 禁止多地登录使用
            /// </summary>
            public string Token { get; set; }

            /// <summary>
            /// 用户登陆时的ip
            /// </summary>
            public string Ip { get; set; }

            
            ///// <summary>
            ///// 这个其实就是appkey
            ///// </summary>
            ///// <returns></returns>
            //public override int GetHashCode()
            //{
            //    return this.Id.GetHashCode() ^ this.LastCheckTime.GetHashCode() ^ this.LoginTime.GetHashCode();
            //}

            /// <summary>
            /// 这个其实就是appkey
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return this.UserToken.GetHashCode() ^ this.Token.GetHashCode();
            }

            ///// <summary>
            ///// 获取 secret值
            ///// </summary>
            ///// <param name="appkey"></param>
            ///// <returns></returns>
            //public static string GetCookieSecret(string appkey)
            //{
            //    MD5 md5 = new MD5CryptoServiceProvider();
            //    byte[] bytes = md5.ComputeHash(Encoding.UTF8.GetBytes("zt" + appkey + "ztt"));
            //    // 第四步：把二进制转化为大写的十六进制
            //    StringBuilder result = new StringBuilder();
            //    for (int i = 0; i < bytes.Length; i++) {
            //        result.Append(bytes[i].ToString("X2"));
            //    }
            //    return result.ToString();
            //}

            ///// <summary>
            ///// 需要填写在配置文件中 在初始化方法中初始化
            ///// </summary>
            //public static string Secret = null;//需要验证

            ///// <summary>
            ///// 编码
            ///// </summary>
            ///// <returns></returns>
            //public string EnCode()
            //{
            //    byte[] keyArray = UTF8Encoding.UTF8.GetBytes(Secret);
            //    var jsonStr = JsonConvert.SerializeObject(this);
            //    byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(jsonStr);
            //    RijndaelManaged rDel = new RijndaelManaged();
            //    rDel.Key = keyArray;
            //    rDel.Mode = CipherMode.ECB;
            //    rDel.Padding = PaddingMode.PKCS7;
            //    ICryptoTransform cTransform = rDel.CreateEncryptor();
            //    byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray,0,toEncryptArray.Length);
            //    return Convert.ToBase64String(resultArray,0,resultArray.Length).Replace("+","%2B").Replace("=","%3D");
            //}

            ///// <summary>
            ///// 解码
            ///// </summary>
            ///// <param name="b64str"></param>
            ///// <returns></returns>
            //public static LoginUser DeCode(string b64str)
            //{
            //    byte[] keyArray = UTF8Encoding.UTF8.GetBytes(Secret);
            //    byte[] toEncryptArray = Convert.FromBase64String(b64str.Replace("%2B","+").Replace("%3D","="));
            //    RijndaelManaged rDel = new RijndaelManaged();
            //    rDel.Key = keyArray;
            //    rDel.Mode = CipherMode.ECB;
            //    rDel.Padding = PaddingMode.PKCS7;
            //    ICryptoTransform cTransform = rDel.CreateDecryptor();
            //    try {
            //        byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray,0,toEncryptArray.Length);
            //        var jsonStr = UTF8Encoding.UTF8.GetString(resultArray);
            //        return JsonConvert.DeserializeObject<LoginUser>(jsonStr);
            //    } catch {
            //        return null;
            //    }
            //}
        }
    }
}