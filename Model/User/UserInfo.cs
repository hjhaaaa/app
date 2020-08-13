using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.User
{
    public class UserInfo
    {   
        [Key]
        public int Id { get; set; }
        public string Mobile { get; set; }
        public string Pwd { get; set; }
        public string PwdMd5 { get; set; }
        public string UserName { get; set; }
        /// <summary>
        /// 0有效 1无效
        /// </summary>
        public int Status { get; set; }
        public string UserToken { get; set; }
        public DateTime CTime { get; set; }
    }
}
