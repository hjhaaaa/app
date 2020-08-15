using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Cache
{
    public class CacheAnnotation
    {
        /// <summary>
        /// 缓存注释
        /// </summary>
        [AttributeUsage(AttributeTargets.Class,AllowMultiple = false,Inherited = true)]
        public class CacheAnnotationAttribute : Attribute
        {
            public String Name { get; set; }

            public CacheAnnotationAttribute(String name)
            {
                this.Name = name;
            }
        }
    }
}
