using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common
{
    /// <summary>
    /// 用途：StackExchange.Redis 助手类
    /// 创建：2019-04-16，Ace Han，创建此助手类
    /// 修订：2019-04-17，Ace Han，连接对象改为单例，同时去掉了写值和取值的方法
    /// </summary>
    public class RedisHelper
    {
        /// <summary>
        /// 获得一个 Redis 数据库
        /// </summary>
        /// <param name="index">Redis 数据库索引</param>
        /// <returns></returns>
        public static IDatabase Database(RedisDbIndex index = RedisDbIndex.Default)
        {
            //ThreadPool.SetMinThreads
            //Hander.GetCounters().Interactive
            return Hander.GetDatabase((Int32)index);
        }

        #region 无序集合

        public static bool SetAdd(string key,string data)
        {
            return Database(RedisDbIndex.Default).SetAdd(key,data);
        }

        public static bool SetRemove(string key,string data)
        {
            return Database(RedisDbIndex.Default).SetRemove(key,data);
        }

        public static bool SetContains(string key,string data)
        {
            return Database(RedisDbIndex.Default).SetContains(key,data);
        }


        #endregion


        #region application
        /// <summary>
        /// 加锁
        /// </summary>
        public static bool LockTake(string key,string data,TimeSpan time)
        {

            return Database(RedisDbIndex.Default).LockTake(key,data,time);
        }
        /// <summary>
        /// 解锁
        /// </summary>
        public static bool LockRelease(string key,string data)
        {
            return Database(RedisDbIndex.Default).LockRelease(key,data);
        }
        /// <summary>
        /// 出列
        /// </summary>
        /// <returns></returns>
        public static string ListLeftPop(string key)
        {
            return Database(RedisDbIndex.Default).ListLeftPop(key);
        }

        /// <summary>
        /// 出列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T ListLeftPop<T>(string key)
        {
            var redisValue = Database(RedisDbIndex.Default).ListLeftPop(key);
            if (!redisValue.HasValue) {
                return default(T);
            }
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(redisValue);
        }

        /// <summary>
        /// 队列总数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long ListLength(string key)
        {
            return Database(RedisDbIndex.Default).ListLength(key);
        }
        /// <summary>
        /// 入列
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ListRightPust(string key,string value)
        {
            return Database(RedisDbIndex.Default).ListRightPush(key,value);
        }

        /// <summary>
        /// 入列
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ListRightPust<T>(string key,T value)
        {
            var jsonValue = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            return Database(RedisDbIndex.Default).ListRightPush(key,jsonValue);
        }

        /// <summary>
        /// 根据Value删除有序集合
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public static long SortedSetRemove(string key,IEnumerable<string> value)
        {
            return Database(RedisDbIndex.Default).SortedSetRemove(key,value.Cast<RedisValue>().ToArray());
        }


        /// <summary>
        /// 根据Value删除有序集合
        /// </summary>
        /// <typeparam name="T">T: class</typeparam>
        /// <param name="key">键值</param>
        /// <param name="value">value</param>
        /// <returns></returns>
        public static bool SortedSetRemove<T>(string key,T value)
        {
            var redisValue = (RedisValue)Newtonsoft.Json.JsonConvert.SerializeObject(value);
            return Database(RedisDbIndex.Default).SortedSetRemove(key,redisValue);
        }
        /// <summary>
        /// 根据Value删除有序集合
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="values">value</param>
        /// <returns></returns>
        public static long SortedSetRemoves<T>(string key,IEnumerable<T> values)
        {
            var redisValues = values.Select(o => (RedisValue)Newtonsoft.Json.JsonConvert.SerializeObject(o)).ToArray();
            return Database(RedisDbIndex.Default).SortedSetRemove(key,redisValues);
        }


        /// <summary>
        /// 根据范围移除SortedSet
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        public static bool SortedSetRemoveRangeByScore(string key,double start,double stop)
        {
            var flag = Database(RedisDbIndex.Default).SortedSetRemoveRangeByScore(key,start,stop);
            return flag > 0;
        }




        /// <summary>
        /// 插入有序集合
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">Json</param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public static bool SortedSetAdd(string key,string value,long sort)
        {
            return Database(RedisDbIndex.Default).SortedSetAdd(key,value,sort);
        }

        public static bool SortedSetAdd<T>(string key,T value,long sort)
        {
            var jsonValue = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            return Database(RedisDbIndex.Default).SortedSetAdd(key,jsonValue,sort);
        }

        /// <summary>
        /// 查询有序队列从小到大
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static IEnumerable<T> SortedSetRangeByScore<T>(string key,long start,long end)
        {
            var redisArry = Database(RedisDbIndex.Default).SortedSetRangeByScore(key,start,end);
            if (redisArry.Count() > 0) {
                return redisArry.Select(o => Newtonsoft.Json.JsonConvert.DeserializeObject<T>(o));
            }
            else {
                return new List<T>();
            }


            // return redisArry.Cast<string>();
        }
        public static long RemoveKey(string key,string value)
        {
            return Database(RedisDbIndex.Default).ListRemove(key,value);
        }
        #endregion
        /// <summary>
        /// 获得一个 Redis 实例
        /// </summary>
        /// <returns></returns>
        public static IServer Server()
        {
            return Hander.GetServer(Hander.GetEndPoints()[0]); // 目前只有一个实例
        }


        public static ConnectionMultiplexer Hander {
            get {
                if (_handler == null) {
                    lock (_locker) {
                        if (_handler != null) {
                            return _handler;
                        }

                        _handler = GetConnectionMultiplexer();
                        return _handler;
                    }
                }

                return _handler;
            }
        }


        static ConnectionMultiplexer GetConnectionMultiplexer()
        {
            var host = ConfigInfo.RedisHost;
            var port = ConfigInfo.RedisPort;
            var password = ConfigInfo.RedisPwd;
            
            var config = new ConfigurationOptions {
                AbortOnConnectFail = false,
                //AllowAdmin = true,
                SyncTimeout = 5000,
                ConnectTimeout = 15000,
                Password = password,
                EndPoints = { $"{host}:{port}" }
            };
            return ConnectionMultiplexer.Connect(config);
        }


        static ConnectionMultiplexer _handler = null;
        static readonly object _locker = new object();




    }


    /// <summary>
    /// Redis 数据库索引
    /// </summary>
    public enum RedisDbIndex : Int32
    {
        /// <summary>默认 FD.Cache</summary>
        Default = 1,
        //One = 1,
        //Two = 2,
        //Three = 3,
        //Four = 4,
        //Five = 5,
        //Six = 6,
        //Seven = 7,
        //Eight = 8,
        //Nine = 9,
        //Ten = 10,
        //Eleven = 11,
        //Twelve = 12,
        //Thirty = 13,
        //Fourteen = 14,
        //Fifteen = 15,
    }
}
