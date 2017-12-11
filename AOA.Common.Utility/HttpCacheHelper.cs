using System;
using Microsoft.Extensions.Caching.Memory;

namespace AOA.Common.Utility
{
    /// <summary>
    /// Cache操作帮助类
    /// </summary>
    public static class HttpCacheHelper
    {

        static MemoryCache Cache = new MemoryCache(new MemoryCacheOptions());

        /// <summary>
        /// 添加Cache, 缓存存在则添加失败
        /// </summary>
        /// <param name="keyValue">键</param>
        /// <param name="objValue">值</param>
        /// <param name="seconds">秒</param>
        public static void Add(string keyValue, object objValue, double seconds)
        {
            Cache.Set(keyValue, objValue, TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// 添加Cache, 缓存存在则添加失败
        /// </summary>
        /// <param name="keyValue">键</param>
        /// <param name="objValue">值</param>
        /// <param name="expireTime">秒</param>
        public static void Add(string keyValue, object objValue, DateTime expireTime)
        {
            Cache.Set(keyValue, objValue, expireTime);
        }

        /// <summary>
        /// 删除缓存
        /// </summary>
        /// <param name="keyValue">键</param>
        public static void Remove(string keyValue)
        {
            Cache.Remove(keyValue);
        }

        /// <summary>
        /// 判断是否存在缓存
        /// </summary>
        /// <param name="keyValue">键</param>
        /// <returns></returns>
        public static bool Exist(string keyValue)
        {
            return Cache.Get(keyValue) != null;
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="keyValue">keyValue</param>
        /// <returns></returns>
        public static object Get(string keyValue)
        {
            return Cache.Get(keyValue);
        }

        #region 获取缓存内容(泛型)

        /// <summary>
        /// 获取缓存内容(泛型)
        /// </summary>
        /// <param name="cacheKey">缓存标识</param>
        /// <returns></returns>
        public static T Get<T>(string cacheKey)
        {
            return Cache.Get<T>(cacheKey);
        }

        #endregion

        #region 添加缓存(泛型)

        /// <summary>
        /// 添加缓存(泛型)
        /// </summary>
        /// <param name="cacheKey">缓存标识</param>
        /// <param name="cacheObj">缓存对象</param>
        /// <param name="seconds">缓存的时间（秒）</param>
        /// <returns></returns>
        public static bool Add<T>(string cacheKey, T cacheObj, double seconds)
        {
            try
            {
                Cache.Set<T>(cacheKey, cacheObj, TimeSpan.FromSeconds(seconds));

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="cacheObj"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        public static bool Add<T>(string cacheKey, T cacheObj, DateTime expireTime)
        {
            try
            {
                Cache.Set<T>(cacheKey, cacheObj, expireTime);

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

    }
}
