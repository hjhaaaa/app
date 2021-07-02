using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using common;

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
            //using (var app = new dbcontext()) {
            //    var logo2 =
            //       app.MDapper.QueryFirstOrDefault<string>("select logo from  applogo  where  status=0");
            //    object logo = "mvcc";
            //    return View(logo);
            //};

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