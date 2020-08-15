using System;
using System.Collections.Generic;
using System.Text;

namespace App.Cache.LoginCache
{
    public class LoginCache : BaseStringCache
    {
        public override int CacheSeconds =>7*24*3600;

        public override string GetKey()
        {
            return CacheHelper.GetKey(this.GetType(),UserToken);
        }
        public string UserToken { get; set; }
        public string LoginToken { get; set; }
        public LoginCache(string UserToken)
        {
            this.UserToken = UserToken;
        }
        public void Set()
        {
            this.Set<LoginCache>(this);
        }
        public LoginCache Get()
        {
            return this.Get<LoginCache>();
        }
    }
}
