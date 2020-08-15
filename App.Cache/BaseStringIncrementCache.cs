using common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Cache
{
    /// <summary>
    /// 递增计数缓存（可设过期时间）
    /// </summary>
    public abstract class BaseStringIncrementCache
    {
        /// <summary>
        /// 缓存时间，秒
        /// </summary>
        public abstract int CacheSeconds { get; }

        /// <summary>
        /// 缓存Key
        /// </summary>
        /// <returns></returns>
        public abstract string GetKey();

        /// <summary>
        /// 添加计数
        /// </summary>
        /// <returns></returns>
        public ulong GetValue()
        {
            var value = RedisHelper.Database().StringGetAsync(this.GetKey()).Result;
            if (value.HasValue) {
                return Convert.ToUInt64(value);
            } else {
                return 0;
            }
        }

        /// <summary>
        /// 添加计数
        /// </summary>
        /// <returns></returns>
        public ulong Increment(int step = 1)
        {
            var value = RedisHelper.Database().StringGetAsync(this.GetKey()).Result;
            if (value.HasValue) {
                var next = Convert.ToInt32(value) + step;
                RedisHelper.Database().StringSetAsync(this.GetKey(),next,TimeSpan.FromSeconds(CacheSeconds));
                return Convert.ToUInt64(next);
            } else {
                RedisHelper.Database().StringSetAsync(this.GetKey(),step,TimeSpan.FromSeconds(CacheSeconds));
                return Convert.ToUInt64(step);
            }
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

    /// <summary>
    /// 递减计数缓存（可设过期时间）
    /// </summary>
    public abstract class BaseStringDecrementCache
    {
        /// <summary>
        /// 缓存时间，秒
        /// </summary>
        public abstract int CacheSeconds { get; }

        /// <summary>
        /// 缓存Key
        /// </summary>
        /// <returns></returns>
        public abstract string GetKey();

        /// <summary>
        /// 添加计数
        /// </summary>
        /// <returns></returns>
        public ulong Decrement(int step = 1)
        {
            var value = RedisHelper.Database().StringGetAsync(this.GetKey()).Result;
            if (value.HasValue) {
                var next = Convert.ToInt32(value) - step;
                RedisHelper.Database().StringSetAsync(this.GetKey(),next,TimeSpan.FromSeconds(CacheSeconds));
                return Convert.ToUInt64(next);
            } else {
                RedisHelper.Database().StringSetAsync(this.GetKey(),step,TimeSpan.FromSeconds(CacheSeconds));
                return Convert.ToUInt64(step);
            }
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
