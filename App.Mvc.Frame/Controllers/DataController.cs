using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace App.Mvc.Frame.Controllers
{   
    
    public class DataController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> GetStr()
        {
            return new string[] { "value1","value2" };
        }

    }
}