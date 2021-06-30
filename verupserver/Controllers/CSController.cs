using common;
using Dapper;
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AppUser.Controllers
{
    [Route("api/[Controller]")]
    public class CSController : ApiController
    {
        [HttpPost]
        public SwatchMResult swatch(SwatchMRep rep)
        {
            using (var app = new dbcontext()) {
                var user = app.MDapper.Query<TbMission>("select * from TbMission where guid=@guid ",new { guid = rep.Guid }).FirstOrDefault();
                if (user == null) {
                    user = new TbMission {
                        AppKey = rep.APPkEY,
                        CurrentMission = 0,
                        Guid = rep.Guid,
                        Ip = rep.Ip,
                        MaxMission = Param.MaxMission,
                        Status = 1,
                        UTime = DateTime.Now
                    };
                    app.MDapper.Update(user);
                    return new SwatchMResult {
                        MaxMission = Param.MaxMission,
                        MissionCount = 0
                    };
                } else {
                    var rsp = new SwatchMResult {
                        MaxMission = Param.MaxMission,
                    };
                    user.Status = 1;
                    user.MaxMission = Param.MaxMission;
                    user.UTime = DateTime.Now;
                    user.CurrentMission = rep.BusyCount;
                    if (Param.NeedDoMission > 0 && rep.BusyCount < Param.MaxMission) {
                        Param.NeedDoMission--;
                        rsp.MaxMission = 1;
                    }
                    app.MDapper.Update(user);
                    return rsp;

                }
            }
        }
        public class TbMission
        {
            [System.ComponentModel.DataAnnotations.Key]
            public int Id { get; set; }
            public string AppKey { get; set; }
            public string Ip { get; set; }
            public string Guid { get; set; }
            public int MaxMission { get; set; }
            public int CurrentMission { get; set; }
            public DateTime UTime { get; set; }
            /// <summary>
            /// 1 ok  2 fail
            /// </summary>
            public int Status { get; set; }
        }
        public class SwatchMResult
        {
            public int MissionCount { get; set; }
            public int MaxMission { get; set; }
        }
        public class SwatchMRep
        {
            public string Ip { get; set; }
            public string Guid { get; set; }
            public int BusyCount { get; set; }
            public string APPkEY { get; set; }
        }
    }
}
