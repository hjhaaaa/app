using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFOModel
{   
    /// <summary>
    /// 用户日记表
    /// </summary>
    public class TbUserNote
    {  
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }
        /// <summary>
        /// 1 删除
        /// </summary>
        public int IsDelete { get; set; }
        public DateTime CTime { get; set; }
        public DateTime UTime { get; set; }
    }
}
