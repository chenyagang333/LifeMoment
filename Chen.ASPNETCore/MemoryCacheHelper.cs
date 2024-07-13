using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chen.ASPNETCore
{
    /// <summary>
    /// 用 ASP.NET Core 的 IMemoryCache 实现的内存缓存。
    /// 封装该 MemoryCacheHelper 的两个原因
    /// 1）使用双精度类型获取随机数，均分请求数据库的时间段，防止同时过期，同时请求数据库，造成请求峰值
    /// 2）由于IEnumerable、IQueryable 等有延迟执行的问题，造成麻烦，因此禁止使用这些类型。
    /// </summary>
    public class MemoryCacheHelper : IMemoryCacheHelper
    {
        private readonly IMemoryCache memoryCache;

        public MemoryCacheHelper(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        private static void ValidateValueType<TResult>()
        {
            // 因为 IEnumerable、IQueryable 等有延迟执行的问题，造成麻烦，因此禁止使用这些类型。
            Type type = typeof(TResult);
            if (type.IsGenericType) // 如果是 IEnumerable<Stiring> 这样的泛型类型，则把 String 这样的具体信息去掉，在比较
            {
                type = type.GetGenericTypeDefinition();
            }
            // 注意用相等比较，不要用 IsAssignableTo 
            if (type == typeof(IEnumerable<>) || type == typeof(IEnumerable)
                || type == typeof(IAsyncEnumerable<TResult>)
                || type == typeof(IQueryable<TResult>) || type == typeof(IQueryable))
            {
                throw new InvalidOperationException($"TResult of {type} is not allowed, please use List<T> or T[] instead.");
            }
        }

        /// <summary>
        /// 返回过期时间 baseExpireSeconds 到 二倍baseExpireSeconds 时间之间随机时间。
        /// </summary>
        /// <param name="cacheEntry"></param>
        /// <param name="baseExpireSeconds">最短过期时间</param>
        private static void InitCacheEntry(ICacheEntry cacheEntry,int baseExpireSeconds)
        {
            double sec = Random.Shared.NextDouble(baseExpireSeconds, baseExpireSeconds * 2);
            TimeSpan expiration = TimeSpan.FromSeconds(sec);
            cacheEntry.AbsoluteExpirationRelativeToNow = expiration;
        }

        public TResult? GetOrCreate<TResult>(string cacheKey, Func<ICacheEntry, TResult?> valueFactory)
        {
            ValidateValueType<TResult>();
            // 因为 IMemoryCache 保存的是一个 CacheEntry，所以 null 也认为是合法的，因此不会有缓存穿透的问题
            return memoryCache.GetOrCreate(cacheKey, valueFactory);
        }

        public TResult? GetOrCreate<TResult>(string cacheKey, Func<ICacheEntry, TResult?> valueFactory, int expireSeconds = 60)
        {
            ValidateValueType<TResult>();
            //因为IMemoryCache保存的是一个CacheEntry，所以null值也认为是合法的，因此返回null不会有“缓存穿透”的问题
            //不调用系统内置的CacheExtensions.GetOrCreate，而是直接用GetOrCreate的代码，这样免得包装一次委托
            if (!memoryCache.TryGetValue(cacheKey,out TResult? result))
            {
                using ICacheEntry entry = memoryCache.CreateEntry(cacheKey);
                InitCacheEntry(entry, expireSeconds);
                result = valueFactory(entry);
                entry.Value = result;
            }
            return result;
        }

        public Task<TResult?> GetOrCreateAsync<TResult>(string cacheKey, Func<ICacheEntry, Task<TResult?>> valueFactory)
        {
            ValidateValueType<TResult>();
            return memoryCache.GetOrCreateAsync(cacheKey, valueFactory);
        }

        public async Task<TResult?> GetOrCreateAsync<TResult>(string cacheKey, Func<ICacheEntry, Task<TResult?>> valueFactory, int expireSeconds = 60)
        {
            ValidateValueType<TResult>();
            //因为IMemoryCache保存的是一个CacheEntry，所以null值也认为是合法的，因此返回null不会有“缓存穿透”的问题
            //不调用系统内置的CacheExtensions.GetOrCreate，而是直接用GetOrCreate的代码，这样免得包装一次委托
            if (!memoryCache.TryGetValue(cacheKey, out TResult? result))
            {
                using ICacheEntry entry = memoryCache.CreateEntry(cacheKey);
                InitCacheEntry(entry, expireSeconds);
                result = await valueFactory(entry);
                entry.Value = result;
            }
            return result;
        }


        public void Remove(string cacheKey)
        {
            memoryCache.Remove(cacheKey);
        }

    }
}
