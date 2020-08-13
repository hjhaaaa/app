using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class versioninfo
    {
        [Key]
        public int id { get; set; }
        public string version { get; set; }
        public int vindex { get; set; }
        /// <summary>
        /// 要操作的文件名
        /// </summary>
        public string file { get; set; }
        /// <summary>
        /// 1 添加/更新  2删除
        /// </summary>
        public int oprtype { get; set; }
        /// <summary>
        /// 下载路由
        /// </summary>
        public string downurl { get; set; }
        public DateTime CTime { get; set; }
    }
}
