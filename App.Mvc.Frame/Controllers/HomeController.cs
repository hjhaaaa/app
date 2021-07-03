using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using common;
using Dapper;

namespace App.Mvc.Frame.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MyView()
        {
            using (var app = new dbcontext()) {
                var logo2 =
                   app.MDapper.QueryFirstOrDefault<string>("sele1ct logo from  applogo  where  status=0");
               
                return View((object)logo2);
            };
            
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}