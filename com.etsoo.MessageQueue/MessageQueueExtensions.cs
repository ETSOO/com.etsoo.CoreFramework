using com.etsoo.Utils.String;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

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
        /// <param name="options">Options</param>
        /// <returns>Result</returns>
        public static IServiceCollection AddMessageQueueProcessor<T>(this IServiceCollection services) where T : class, IMessageQueueProcessor
        {
            services.AddSingleton<IMessageQueueProcessor, T>();
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
    }
}
