using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace com.etsoo.Utils.Serialization
{
    /// <summary>
    /// Cache factory
    /// 缓存工厂
    /// </summary>
    public static class CacheFactory
    {
        /// <summary>
        /// Cache object
        /// </summary>
        /// <typeparam name="T">Generic return type</typeparam>
        /// <param name="cache">Cache interface</param>
        /// <param name="cacheHours">Cache hours, zero and negative value means disable the feature</param>
        /// <param name="keyFunc">Key generation function</param>
        /// <param name="actionFunc">Value generaction function</param>
        /// <param name="options">Options</param>
        /// <returns>Result</returns>
        public static async Task<T?> DoAsync<T>(
            IDistributedCache cache,
            double cacheHours,
            Func<string> keyFunc,
            Func<Task<T?>> actionFunc,
            DistributedCacheEntryOptions? options = null)
        {
            if (cacheHours <= 0)
            {
                return await actionFunc();
            }

            var key = keyFunc();

            var cachedBytes = await cache.GetAsync(key);
            if (cachedBytes is not null && cachedBytes.Any())
            {
                return await JsonSerializer.DeserializeAsync<T>(SharedUtils.GetStream(cachedBytes));
            }

            var newValue = await actionFunc();

            options ??= new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(cacheHours)
            };
            await cache.SetAsync(key, JsonSerializer.SerializeToUtf8Bytes(newValue), options);

            return newValue;
        }

        /// <summary>
        /// Cache bytes
        /// </summary>
        /// <param name="cache">Cache interface</param>
        /// <param name="cacheHours">Cache hours, zero and negative value means disable the feature</param>
        /// <param name="keyFunc">Key generation function</param>
        /// <param name="actionFunc">Value generaction function</param>
        /// <param name="options">Options</param>
        /// <returns>Task</returns>
        public static async Task<(ReadOnlyMemory<byte>, bool)> DoBytesAsync(
            IDistributedCache cache,
            double cacheHours,
            Func<string> keyFunc,
            Func<Task<ReadOnlyMemory<byte>>> actionFunc,
            DistributedCacheEntryOptions? options = null)
        {
            if (cacheHours <= 0)
            {
                return (await actionFunc(), false);
            }

            var key = keyFunc();

            var cachedValue = await cache.GetAsync(key);
            if (cachedValue is not null && cachedValue.Any())
            {
                return (cachedValue, false);
            }

            var newValue = await actionFunc();

            options ??= new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(cacheHours)
            };
            await cache.SetAsync(key, newValue.ToArray(), options);

            return (newValue, true);
        }

        /// <summary>
        /// Cache string
        /// </summary>
        /// <param name="cache">Cache interface</param>
        /// <param name="cacheHours">Cache hours, zero and negative value means disable the feature</param>
        /// <param name="keyFunc">Key generation function</param>
        /// <param name="actionFunc">Value generaction function</param>
        /// <param name="options">Options</param>
        /// <returns>Result</returns>
        public static async Task<string> DoStringAsync(
            IDistributedCache cache,
            double cacheHours,
            Func<string> keyFunc,
            Func<Task<string>> actionFunc,
            DistributedCacheEntryOptions? options = null)
        {
            if (cacheHours <= 0)
            {
                return await actionFunc();
            }

            var key = keyFunc();

            var cachedValue = await cache.GetStringAsync(key);
            if (!string.IsNullOrEmpty(cachedValue))
            {
                return cachedValue;
            }

            var newValue = await actionFunc();

            options ??= new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(cacheHours)
            };
            await cache.SetStringAsync(key, newValue, options);

            return newValue;
        }
    }
}
