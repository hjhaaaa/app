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
            Exception lastError = Server.GetLastError();//��ȡ�쳣
            if (lastError != null) {
                //�쳣��Ϣ
                string strExceptionMessage = string.Empty;

                //��HTTP 404�����⴦����������ȫ������500����������
                HttpException httpError = lastError as HttpException;
                if (httpError != null) {
                    //��ȡ�������
                    int httpCode = httpError.GetHttpCode();
                    //��ȡ�쳣��Ϣ
                    strExceptionMessage = httpError.Message;
                    if (httpCode == 400 || httpCode == 404) {
                        Response.StatusCode = 404;
                        //��ת��ָ���ľ�̬404��Ϣҳ��
                        Response.WriteFile("~/404.html");
                        //һ��Ҫ����Server.ClearError()����ᴥ����������ҳ�����ǻ�ҳ��
                        Server.ClearError();
                        return;
                    }
                }
                strExceptionMessage = lastError.Message;//�õ�������Ϣ������д����־��

                /*-----------------------------------------------------
                 * �˴�����ɸ������������־��¼�����ߴ�������ҵ������
                 * ---------------------------------------------------*/
                /* ��ת����̬ҳ��һ��Ҫ��Response.WriteFile����  */

                Response.StatusCode = 500;
                Response.Write("~/error.html");
                //һ��Ҫ����Server.ClearError()����ᴥ����������ҳ�����ǻ�ҳ��
                Server.ClearError();
            }


        }
    }
}
