using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chen.ASPNETCore
{
    public interface IMemoryCacheHelper
    {
        /// <summary>
        /// 从缓存中获取数据，如果缓存中没有数据，则调用 valueFactory 获取数据。
        /// 可以用 AOP + Attribute 的方式来修饰到 Service 接口中实现缓存，更加优美，但是没有这种方式灵活。
        /// 默认最长的缓存过期事件是 expireSeconds 秒，当然也可以在领域事件的 Handle 中调用 Update 更新缓存，
        /// 或调用 Remove 删除缓存。
        /// 由于 IMemoryCache 会把 null 当成合法的值，因此不会有缓存穿透的问题，但是还是建议用我这里封装的
        /// ICacheHelper，原因如下：
        /// 1）可以切换别的实现类，比如可以保存到 MemCached、Redis 等地方。这样可以隔离变化。
        /// 2）IMemoryCache 的 valueFactory 用起来比较麻烦，还要单独声明一个 ICacheEntry 参数，大部分时间用不到该参数。
        /// 3）这里把 expireSeconds 加上了一个随机偏差，这样可以避免短时间内同样的请求集中过期导致缓存雪崩的问题。
        /// 4）这里加入了缓存数据类型不能是 IEnumerable、IQueryable 等类型的限制。 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="valueFactory"></param>
        /// <param name="expireSeconds"></param>
        /// <returns></returns>
        TResult? GetOrCreate<TResult>(string cacheKey, Func<ICacheEntry, TResult?> valueFactory, int expireSeconds = 60);

        TResult? GetOrCreate<TResult>(string cacheKey, Func<ICacheEntry, TResult?> valueFactory);

        Task<TResult?> GetOrCreateAsync<TResult>(string cacheKey, Func<ICacheEntry, Task<TResult?>> valueFactory, int expireSeconds = 60);

        Task<TResult?> GetOrCreateAsync<TResult>(string cacheKey, Func<ICacheEntry, Task<TResult?>> valueFactory);

        /// <summary>
        /// 删除缓存的值
        /// </summary>
        /// <param name="cacheKey"></param>
        void Remove(string cacheKey);
    }
}
