using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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
        /// <summary>
        /// 验证手机合法性
        /// </summary>
        /// <param name="s">手机号</param>
        /// <returns></returns>
        public static bool IsMobile(this string s)
        {
            if (s.NullOrEmpty())
                return false;
            var reg =
                new Regex(
                    @"^1\d{10}$");
            return reg.IsMatch(s);
        }
        public static bool NullOrEmpty(this string str)
        {
            if (str != null) {
                //str = str.Trim(new[] { '\t', '\r', '\n', ' ' });
                str = str.Trim();
            }
            return string.IsNullOrEmpty(str);
        }
        public static bool IsPassword(this string s)
        {
            if (s != null)
                s = s.Trim();
            if (s.NullOrEmpty())
                return false;
            //^.{6,20}$  密码长度
            var reg = new Regex("^(?![0-9]+$)(?![a-zA-Z]+$)[0-9A-Za-z]{6,20}$");
            return reg.IsMatch(s);
        }
    }
}
