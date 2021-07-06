using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.DTO
{
    public interface IWebApiResult { 
      int Code { get; set; }
        string Msg { get; set; }
    }

    public class WebApiResult : IWebApiResult
    {
        public int Code { get; set; } 
        public string Msg { get; set; } 
        //public object Data { get; set; }
        public static WebApiResult Success()
        {
            return new WebApiResult { Code = 0,Msg = "Ok" };
        }
        public static WebApiResult Error(string msg= "error")
        {
            return new WebApiResult { Code = 1,Msg = msg };
        }
    }
    public class WebApiResult<T>: WebApiResult 
    {
        public T Data { get;set;
        }
        public static WebApiResult<T> Success( T data)
        {
            return new WebApiResult<T> { Code = 0,Msg = "Ok",Data =data};
        }
       
    }
}
