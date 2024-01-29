using Microsoft.Extensions.Caching.Distributed;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

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
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        [RequiresDynamicCode("DoAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("DoAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        public static async Task<T?> DoAsync<T>(
            IDistributedCache cache,
            double cacheHours,
            Func<string> keyFunc,
            Func<Task<T?>> actionFunc,
            DistributedCacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            if (cacheHours <= 0)
            {
                return await actionFunc();
            }

            var key = keyFunc();

            var cachedBytes = await cache.GetAsync(key, cancellationToken);
            if (cachedBytes is not null && cachedBytes.Length > 0)
            {
                return await JsonSerializer.DeserializeAsync<T>(SharedUtils.GetStream(cachedBytes), cancellationToken: cancellationToken);
            }

            var newValue = await actionFunc();
            if (newValue is null)
            {
                await cache.RemoveAsync(key, cancellationToken);
            }
            else
            {
                options ??= new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(cacheHours)
                };
                await cache.SetAsync(key, JsonSerializer.SerializeToUtf8Bytes(newValue), options, cancellationToken);
            }

            return newValue;
        }

        /// <summary>
        /// Cache object
        /// </summary>
        /// <typeparam name="T">Generic return type</typeparam>
        /// <param name="cache">Cache interface</param>
        /// <param name="cacheHours">Cache hours, zero and negative value means disable the feature</param>
        /// <param name="keyFunc">Key generation function</param>
        /// <param name="actionFunc">Value generaction function with Json type info</param>
        /// <param name="typeInfo">Json type info</param>
        /// <param name="options">Options</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public static async Task<T?> DoAsync<T>(
            IDistributedCache cache,
            double cacheHours,
            Func<string> keyFunc,
            Func<JsonTypeInfo<T>, Task<T?>> actionFunc,
            JsonTypeInfo<T> typeInfo,
            DistributedCacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            if (cacheHours <= 0)
            {
                return await actionFunc(typeInfo);
            }

            var key = keyFunc();

            var cachedBytes = await cache.GetAsync(key, cancellationToken);
            if (cachedBytes is not null && cachedBytes.Length > 0)
            {
                return await JsonSerializer.DeserializeAsync(SharedUtils.GetStream(cachedBytes), typeInfo, cancellationToken: cancellationToken);
            }

            var newValue = await actionFunc(typeInfo);

            if (newValue is null)
            {
                await cache.RemoveAsync(key, cancellationToken);
            }
            else
            {
                options ??= new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(cacheHours)
                };
                await cache.SetAsync(key, JsonSerializer.SerializeToUtf8Bytes(newValue, typeInfo), options, cancellationToken);
            }

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
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public static async Task<(ReadOnlyMemory<byte>, bool)> DoBytesAsync(
            IDistributedCache cache,
            double cacheHours,
            Func<string> keyFunc,
            Func<Task<ReadOnlyMemory<byte>>> actionFunc,
            DistributedCacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            if (cacheHours <= 0)
            {
                return (await actionFunc(), false);
            }

            var key = keyFunc();

            var cachedValue = await cache.GetAsync(key, cancellationToken);
            if (cachedValue is not null && cachedValue.Length > 0)
            {
                return (cachedValue, false);
            }

            var newValue = await actionFunc();

            options ??= new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(cacheHours)
            };
            await cache.SetAsync(key, newValue.ToArray(), options, cancellationToken);

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
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public static async Task<string> DoStringAsync(
            IDistributedCache cache,
            double cacheHours,
            Func<string> keyFunc,
            Func<Task<string>> actionFunc,
            DistributedCacheEntryOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            if (cacheHours <= 0)
            {
                return await actionFunc();
            }

            var key = keyFunc();

            var cachedValue = await cache.GetStringAsync(key, cancellationToken);
            if (!string.IsNullOrEmpty(cachedValue))
            {
                return cachedValue;
            }

            var newValue = await actionFunc();

            options ??= new DistributedCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromHours(cacheHours)
            };
            await cache.SetStringAsync(key, newValue, options, cancellationToken);

            return newValue;
        }
    }
}
