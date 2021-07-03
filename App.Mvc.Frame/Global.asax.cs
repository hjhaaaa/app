using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace App.Mvc.Frame
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Application_Error(Object sender,EventArgs e)
        {
            Exception lastError = Server.GetLastError();//获取异常
            if (lastError != null) {
                //异常信息
                string strExceptionMessage = string.Empty;

                //对HTTP 404做额外处理，其他错误全部当成500服务器错误
                HttpException httpError = lastError as HttpException;
                if (httpError != null) {
                    //获取错误代码
                    int httpCode = httpError.GetHttpCode();
                    //获取异常信息
                    strExceptionMessage = httpError.Message;
                    if (httpCode == 400 || httpCode == 404) {
                        Response.StatusCode = 404;
                        //跳转到指定的静态404信息页面
                        Response.WriteFile("~/404.html");
                        //一定要调用Server.ClearError()否则会触发错误详情页（就是黄页）
                        Server.ClearError();
                        return;
                    }
                }
                strExceptionMessage = lastError.Message;//得到错误信息，可以写到日志里

                /*-----------------------------------------------------
                 * 此处代码可根据需求进行日志记录，或者处理其他业务流程
                 * ---------------------------------------------------*/
                /* 跳转到静态页面一定要用Response.WriteFile方法  */

                Response.StatusCode = 500;
                Response.Write("~/error.html");
                //一定要调用Server.ClearError()否则会触发错误详情页（就是黄页）
                Server.ClearError();
            }


        }
    }
}
