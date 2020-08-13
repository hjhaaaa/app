using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace common.Tool
{
    public static class StringTool
    {
        public static string Md5Deal (this string str)
        {
            MD5 mD5 = MD5.Create();
            byte[] by = System.Text.Encoding.UTF8.GetBytes(str);
            string pwd = "";
            for (int i = 0; i < by.Length; i++) {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符

                pwd = pwd + by[i].ToString("X");

            }
            return pwd;
           
        }
    }
}
