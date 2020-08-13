using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Model;

namespace main
{
    class HttpClientReq
    {
        public static string Req(string method,Dictionary<string,string> ps,string route)
        {
            using (HttpClient hc = new HttpClient())
            {
                if(method=="g")
                {
                    string parms = "";
                    foreach(var v in ps)
                    {
                        parms += v.Key + "=" + v.Value+"&";
                    }
                   parms= parms.TrimEnd('&');
                   var respos=  hc.GetAsync(Param.serviceurl+route+"?"+parms ).Result;
                    if(respos.IsSuccessStatusCode)
                    {
                        string a = respos.Content.ReadAsStringAsync().Result;
                      
                        return a;
                    }
                }
                else if(method=="p")
                {
                    FormUrlEncodedContent content = new FormUrlEncodedContent(ps.ToList());
                    var respos = hc.PostAsync(Param.serviceurl+route,content).Result;
                    if (respos.IsSuccessStatusCode)
                    {
                        return respos.Content.ReadAsStringAsync().Result;
                    }
                }
                return "fail";
            }
        }
    }
}
