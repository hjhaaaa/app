
using App.DTO;
using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace App.EF.Api.Controllers
{
    public class EFController : ApiController
    {
        [HttpGet]

        public IWebApiResult EfTest()
        {
            var efUser = new TbEfUser { Name = "hjh",Address = "123" };
            
            using (var app = new Model()) {
                app.tbEfUsers.Add(efUser);
                 app.SaveChanges();
            }
            return WebApiResult.Success();
        }
    }
}
