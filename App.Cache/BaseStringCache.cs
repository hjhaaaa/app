
using common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Cache
{
    /// <summary>
    /// 字符串或对象缓存
    /// </summary>
    public abstract class BaseStringCache
    {
        /// <summary>
        /// 缓存时间（秒）
        /// </summary>
        public abstract int CacheSeconds { get; }

        /// <summary>
        /// 缓存key
        /// </summary>
        /// <returns></returns>
        public abstract string GetKey();

        /// <summary>
        /// 获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected T Get<T>()
        {
            var value = RedisHelper.Database().StringGetAsync(this.GetKey()).Result;
            if (value.HasValue) {
                return CacheHelper.ToObject<T>(value);
            }
            return default(T);
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        protected bool Set<T>(T t)
        {
            return this.Set(t,TimeSpan.FromSeconds(this.CacheSeconds));
        }

        protected bool Set<T>(T t,Int32 seconds)
        {
            return this.Set(t,TimeSpan.FromSeconds(seconds));
        }

        private bool Set<T>(T t,TimeSpan expiry)
        {
            var value = CacheHelper.ToJson<T>(t);
            return RedisHelper.Database().StringSetAsync(this.GetKey(),value,expiry).Result;
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <returns></returns>
        public virtual bool Remove()
        {
            return RedisHelper.Database().KeyDeleteAsync(this.GetKey()).Result;
        }

    }
}
