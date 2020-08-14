﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace common
{
    public class Webapiresult
    {
        public webapicode code { get; set; }
        public string msg { get; set; }
    }
    public enum webapicode
    {
        ok=1,
        fail=2
    }
    public class WebapiresultM: Webapiresult
    {
       
        public string Data { get; set; }
        public string Data2 { get; set; }
    }
}