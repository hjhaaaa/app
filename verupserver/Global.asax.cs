using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Routing;

namespace verupserver
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            #region 返回数据格式化说明
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
            var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            json.SerializerSettings.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat;
            json.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
            json.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Local;
            json.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            json.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.None;
            json.SerializerSettings.ContractResolver = new DefaultContractResolver();
            #endregion
           
           // GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            GlobalConfiguration.Configure(o => {
                o.EnableCors(new EnableCorsAttribute("*","*","*"));
                o.MapHttpAttributeRoutes();
                o.Routes.MapHttpRoute(
                   name: "DefaultApi",
                   routeTemplate: "api/{controller}/{action}/{id}",
                   defaults: new { id = RouteParameter.Optional }
               );
            });
        }
    }
}
