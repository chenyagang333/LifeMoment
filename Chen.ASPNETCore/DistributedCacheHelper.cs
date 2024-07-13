using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chen.ASPNETCore
{
    public class DistributedCacheHelper : IDistributedCacheHelper
    {
        private readonly IDistributedCache distributedCache;

        public DistributedCacheHelper(IDistributedCache distributedCache)
        {
            this.distributedCache = distributedCache;
        }

        private static DistributedCacheEntryOptions CreateOptions(int baseExpireSeconds)
        {
            double sec = Random.Shared.NextDouble(baseExpireSeconds, baseExpireSeconds * 2);
            TimeSpan expiration = TimeSpan.FromSeconds(sec);
            DistributedCacheEntryOptions options = new();
            options.AbsoluteExpirationRelativeToNow = expiration;
            return options;
        }

        public TResult? GetOrCreate<TResult>(string cacheKey, Func<DistributedCacheEntryOptions, TResult?> valueFactory)
        {
            string? jsonStr = distributedCache.GetString(cacheKey);
            // 缓存中不存在
            if (string.IsNullOrEmpty(jsonStr))
            {
                DistributedCacheEntryOptions options = new();
                TResult? result = valueFactory(options); // 如果数据源中也没有查到，可能返回 null
                // null 会被 json 反序列化为字符串“null”，所以可以防范缓存穿透。
                string jsonOfResult = JsonSerializer.Serialize(result, typeof(TResult));
                distributedCache.SetString(cacheKey, jsonOfResult, options);
                return result;
            }
            else
            {
                distributedCache.Refresh(cacheKey); // 刷新，以便于滑动过期时间延期。
                return JsonSerializer.Deserialize<TResult>(jsonStr);
            }
        }

        public TResult? GetOrCreate<TResult>(string cacheKey, Func<DistributedCacheEntryOptions, TResult?> valueFactory, int expireSeconds = 60)
        {
            string? jsonStr = distributedCache.GetString(cacheKey);
            // 缓存中不存在
            if (string.IsNullOrEmpty(jsonStr))
            {
                var options = CreateOptions(expireSeconds);
                TResult? result = valueFactory(options); // 如果数据源中也没有查到，可能返回 null
                // null 会被 json 反序列化为字符串“null”，所以可以防范缓存穿透。
                string jsonOfResult = JsonSerializer.Serialize(result,typeof(TResult));
                distributedCache.SetString(cacheKey, jsonOfResult, options);
                return result;
            }
            else
            {
                // "null" 会被反序列化为 null
                // TResult 如果是引用类型，就有为 null 的可能性，如果 TResult 是值类型、
                // 在写入的时候肯定写入的是0、1之类的值，反序列化出来不会是 null
                // 所以如果 obj 这里为 null，那么存进去的时候一定是引用类型
                distributedCache.Refresh(cacheKey); // 刷新，以便于滑动过期时间延期。
                return JsonSerializer.Deserialize<TResult>(jsonStr);
            }
        }

        public async Task<TResult?> GetOrCreateAsync<TResult>(string cacheKey, Func<DistributedCacheEntryOptions, Task<TResult?>> valueFactory)
        {
            string? jsonStr = distributedCache.GetString(cacheKey);
            if (string.IsNullOrEmpty(jsonStr))
            {
                DistributedCacheEntryOptions options = new();
                TResult? result = await valueFactory(options); // 如果数据源中也没有查到，可能返回 null
                string jsonOfResult = JsonSerializer.Serialize(result, typeof(TResult));
                await distributedCache.SetStringAsync(cacheKey, jsonOfResult, options);
                return result;
            }
            else
            {
                await distributedCache.RefreshAsync(cacheKey); // 刷新，以便于滑动过期时间延期。
                return JsonSerializer.Deserialize<TResult>(jsonStr);
            }
        }

        public async Task<TResult?> GetOrCreateAsync<TResult>(string cacheKey, Func<DistributedCacheEntryOptions, Task<TResult?>> valueFactory, int expireSeconds = 60)
        {
            string? jsonStr = distributedCache.GetString(cacheKey);
            if (string.IsNullOrEmpty(jsonStr))
            {
                var options = CreateOptions(expireSeconds);
                TResult? result = await valueFactory(options); // 如果数据源中也没有查到，可能返回 null
                string jsonOfResult = JsonSerializer.Serialize(result, typeof(TResult));
                await distributedCache.SetStringAsync(cacheKey, jsonOfResult, options);
                return result;
            }
            else
            {
                await distributedCache.RefreshAsync(cacheKey); // 刷新，以便于滑动过期时间延期。
                return JsonSerializer.Deserialize<TResult>(jsonStr);
            }
        }

        public void Remove(string cackeKey)
        {
            distributedCache.Remove(cackeKey);
        }

        public Task RemoveAsync(string cackeKey)
        {
            return distributedCache.RemoveAsync(cackeKey);
        }
    }
}
