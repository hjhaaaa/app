using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace App.Cache
{
    public class CacheHelper
    {
        /// <summary>
        /// 获得统一格式的缓存 Key
        /// </summary>
        /// <param name="type">缓存类的类型</param>
        /// <param name="key">缓存的业务 Key，即构造函数的实参</param>
        /// <returns></returns>
        public static String GetKey(Type type,String key)
        {
            return $"{type.FullName}:{key}".Replace(".",":");
        }

        /// <summary>
        /// 对象转 JSON 字符串
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">对象</param>
        /// <returns></returns>
        public static String ToJson<T>(T value)
        {
            return value is String ? value.ToString() : JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// JSON(RedisValue) 转对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">RedisValue</param>
        /// <returns></returns>
        public static T ToObject<T>(RedisValue value)
        {
            if (!value.HasValue)
                return default(T);
            if (typeof(T).Name != typeof(String).Name)
                return JsonConvert.DeserializeObject<T>(value);

            return JsonConvert.DeserializeObject<T>($"'{value}'");
        }

        /// <summary>
        /// RedisValue转List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static List<T> ConvertList<T>(RedisValue[] values)
        {
            List<T> result = new List<T>();
            if (values != null && values.Length > 0) {
                foreach (var item in values) {
                    var model = ToObject<T>(item);
                    result.Add(model);
                }
            }
            return result;
        }

        /// <summary>
        /// RedisValue转List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static List<T> ConvertList<T>(IEnumerable<RedisValue> values)
        {
            List<T> result = new List<T>();
            if (values != null && values.Any()) {
                foreach (var item in values) {
                    var model = ToObject<T>(item);
                    result.Add(model);
                }
            }
            return result;
        }

        /// <summary>
        /// RedisValue转HashSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static HashSet<T> ConvertHashSet<T>(RedisValue[] values)
        {
            HashSet<T> result = new HashSet<T>();
            if (values != null && values.Length > 0) {
                foreach (var item in values) {
                    var model = ToObject<T>(item);
                    result.Add(model);
                }
            }
            return result;
        }

        /// <summary>
        /// RedisValue转HashSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static HashSet<T> ConvertHashSet<T>(IEnumerable<RedisValue> values)
        {
            HashSet<T> result = new HashSet<T>();
            if (values != null && values.Any()) {
                foreach (var item in values) {
                    var model = ToObject<T>(item);
                    result.Add(model);
                }
            }
            return result;
        }

        /// <summary>
        /// SortedSetEntry转List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static List<KeyValuePair<T,double>> ConvertList<T>(SortedSetEntry[] values)
        {
            List<KeyValuePair<T,double>> result = new List<KeyValuePair<T,double>>();
            if (values != null && values.Length > 0) {
                foreach (var item in values) {
                    var value = ToObject<T>(item.Element);
                    result.Add(new KeyValuePair<T,double>(value,item.Score));
                }
            }
            return result;
        }

        /// <summary>
        /// 对象转 Hash
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static HashEntry[] ToHashEntries<T>(T obj)
        {
            try {
                PropertyInfo[] properties = obj.GetType().GetProperties();
                return properties
                .Select(property => {
                    String hashValue = string.Empty;
                    var propertyValue = property.GetValue(obj);
                    var propertyType = property.PropertyType;
                    if (propertyType.IsEnum) {
                        var vals = Enum.GetValues(propertyType);
                        foreach (int i in vals) {
                            var enumName = propertyType.GetEnumName(i);
                            if (enumName == propertyValue.ToString()) {
                                hashValue = i.ToString();
                                break;
                            }
                        }
                    } else if ((propertyType.IsGenericType || propertyType.IsArray || propertyType.IsClass) && propertyType.Name != "String") {
                        hashValue = JsonConvert.SerializeObject(propertyValue);
                    } else {
                        hashValue = propertyValue.ToString();
                    }
                    return new HashEntry(property.Name,hashValue);
                })
                .ToArray();
            } catch (Exception ex) {
                return null;
            }
        }

        /// <summary>
        /// Hash转对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashEntries"></param>
        /// <returns></returns>
        public static T ToObject<T>(HashEntry[] hashEntries)
        {
            var obj = Activator.CreateInstance<T>();
            try {
                PropertyInfo[] properties = obj.GetType().GetProperties();
                var dic = hashEntries.ToDictionary(o => o.Name);
                foreach (var property in properties) {
                    var propertyName = property.Name;
                    if (dic.ContainsKey(propertyName)) {
                        var value = dic[propertyName].Value;
                        var propertyType = property.PropertyType;
                        if (propertyType.IsInterface || propertyType.IsAbstract) {
                            continue;
                        } else if (propertyType.IsEnum) {
                            property.SetValue(obj,Convert.ChangeType(Enum.Parse(property.PropertyType,value),property.PropertyType));
                        } else if ((propertyType.IsGenericType || propertyType.IsArray || propertyType.IsClass) && propertyType.Name != "String") {
                            property.SetValue(obj,Convert.ChangeType(JsonConvert.DeserializeObject(value,property.PropertyType),property.PropertyType));
                        } else {
                            switch (propertyType.Name) {
                                case "String": property.SetValue(obj,value.ToString(),null); break;
                                case "Char": property.SetValue(obj,Char.Parse(value),null); break;
                                case "Boolean": property.SetValue(obj,Boolean.Parse(value),null); break;
                                case "Byte": property.SetValue(obj,Byte.Parse(value),null); break;
                                case "SByte": property.SetValue(obj,SByte.Parse(value),null); break;
                                case "Decimal": property.SetValue(obj,Decimal.Parse(value),null); break;
                                case "Double": property.SetValue(obj,Double.Parse(value),null); break;
                                case "DateTime": property.SetValue(obj,DateTime.Parse(value),null); break;
                                case "Int16": property.SetValue(obj,Int16.Parse(value),null); break;
                                case "Int32": property.SetValue(obj,Int32.Parse(value),null); break;
                                case "Int64": property.SetValue(obj,Int64.Parse(value),null); break;
                                case "Single": property.SetValue(obj,Single.Parse(value),null); break;
                                case "Entity":; break;
                                default: property.SetValue(obj,Convert.ChangeType(value,property.PropertyType)); break;
                            }
                        }
                    }
                }
                return obj;
            } catch (Exception ex) {
                return obj;
            }
        }
    }
}
