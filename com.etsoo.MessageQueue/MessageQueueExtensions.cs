using com.etsoo.MessageQueue.QueueProcessors;
using com.etsoo.Utils.String;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Serialization.Metadata;

namespace com.etsoo.MessageQueue
{
    /// <summary>
    /// Message queue extensions
    /// 消息队列扩展
    /// </summary>
    public static class MessageQueueExtensions
    {
        /// <summary>
        /// Add message queue processor to service collection
        /// 将消息队列处理器添加到服务集合
        /// </summary>
        /// <param name="services">Services</param>
        /// <returns>Result</returns>
        public static IServiceCollection AddMessageQueueProcessor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this IServiceCollection services) where T : class, IMessageQueueProcessor
        {
            services.AddSingleton<IMessageQueueProcessor, T>();
            return services;
        }

        /// <summary>
        /// Add message queue processor to service collection
        /// 将消息队列处理器添加到服务集合
        /// </summary>
        /// <param name="services">Services</param>
        /// <param name="implementationFactory">Implementation factory</param>
        /// <returns>Result</returns>
        public static IServiceCollection AddMessageQueueProcessor(this IServiceCollection services, Func<IServiceProvider, IMessageQueueProcessor> implementationFactory)
        {
            services.AddSingleton(implementationFactory);
            return services;
        }

        /// <summary>
        /// Get header value
        /// 获取标头值
        /// </summary>
        /// <param name="headers">Headers collection</param>
        /// <param name="key">Item key</param>
        /// <returns>Result</returns>
        public static string? GetHeaderValue(this IDictionary<string, object> headers, string key)
        {
            if (headers.TryGetValue(key, out var value) && value != null)
            {
                if (value is byte[] bytes)
                {
                    return Encoding.UTF8.GetString(bytes);
                }
                else
                {
                    return Convert.ToString(value);
                }
            }

            return default;
        }

        /// <summary>
        /// Get header value
        /// 获取标头值
        /// </summary>
        /// <typeparam name="T">Generic value type</typeparam>
        /// <param name="headers">Headers collection</param>
        /// <param name="key">Item key</param>
        /// <returns>Result</returns>
        public static T? GetHeaderValue<T>(this IDictionary<string, object> headers, string key) where T : struct
        {
            if (headers.TryGetValue(key, out var value) && value != null)
            {
                if (value is byte[] bytes)
                {
                    return StringUtils.TryParse<T>(Encoding.UTF8.GetString(bytes));
                }
                else
                {
                    return StringUtils.TryParseObject<T>(value);
                }
            }

            return default;
        }

        /// <summary>
        /// Async model to JSON bytes
        /// 异步模型到 JSON 字节
        /// </summary>
        /// <typeparam name="T">Generic model type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        [RequiresDynamicCode("ToJsonBytesAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("ToJsonBytesAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        public static async Task<ReadOnlyMemory<byte>> ToJsonBytesAsync<T>(this T model, CancellationToken cancellationToken = default) where T : IMessageQueueMessage
        {
            return await MessageQueueUtils.ToJsonBytesAsync(model, cancellationToken);
        }

        /// <summary>
        /// Async model to JSON bytes
        /// 异步模型到 JSON 字节
        /// </summary>
        /// <typeparam name="T">Generic model type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="typeInfo">Json type info</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public static async Task<ReadOnlyMemory<byte>> ToJsonBytesAsync<T>(this T model, JsonTypeInfo<T> typeInfo, CancellationToken cancellationToken = default) where T : IMessageQueueMessage
        {
            return await MessageQueueUtils.ToJsonBytesAsync(model, typeInfo, cancellationToken);
        }

        /// <summary>
        /// To JSON string
        /// 转化为 JSON 字符串
        /// </summary>
        /// <param name="bytes">JSON bytes</param>
        /// <returns>Result</returns>
        public static string ToJsonString(this ReadOnlyMemory<byte> bytes)
        {
            return Encoding.UTF8.GetString(bytes.Span);
        }

        /// <summary>
        /// Async JSON bytes to model
        /// 异步 JSON 字节转化为模型
        /// </summary>
        /// <typeparam name="T">Generic model type</typeparam>
        /// <param name="bytes">JSON bytes</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        [RequiresDynamicCode("ToMessageAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("ToMessageAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        public static async ValueTask<T?> ToMessageAsync<T>(this ReadOnlyMemory<byte> bytes, CancellationToken cancellationToken = default) where T : class
        {
            return await MessageQueueUtils.FromJsonBytesAsync<T>(bytes, cancellationToken);
        }

        /// <summary>
        /// Async JSON bytes to model
        /// 异步 JSON 字节转化为模型
        /// </summary>
        /// <typeparam name="T">Generic model type</typeparam>
        /// <param name="bytes">JSON bytes</param>
        /// <param name="typeInfo">Json type info</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        public static async ValueTask<T?> ToMessageAsync<T>(this ReadOnlyMemory<byte> bytes, JsonTypeInfo<T> typeInfo, CancellationToken cancellationToken = default) where T : class
        {
            return await MessageQueueUtils.FromJsonBytesAsync(bytes, typeInfo, cancellationToken);
        }
    }
}
