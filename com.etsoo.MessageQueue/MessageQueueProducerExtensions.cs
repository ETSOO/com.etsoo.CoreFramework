using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Metadata;

namespace com.etsoo.MessageQueue
{
    /// <summary>
    /// Message queue producer extensions
    /// 消息队列生产者扩展
    /// </summary>
    public static class MessageQueueProducerExtensions
    {
        /// <summary>
        /// Async send message model
        /// 异步发送信息模式
        /// </summary>
        /// <typeparam name="T">General model type</typeparam>
        /// <param name="producer">Producer</param>
        /// <param name="model">Message model</param>
        /// <param name="properties">Properties</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Message id</returns>
        [RequiresDynamicCode("SendJsonAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        [RequiresUnreferencedCode("SendJsonAsync 'T' may require dynamic access otherwise can break functionality when trimming application code")]
        public static async Task<string> SendJsonAsync<T>(this IMessageQueueProducer producer, T model, MessageProperties? properties = null, CancellationToken cancellationToken = default)
        {
            var bytes = await MessageQueueUtils.ToJsonBytesAsync(model, cancellationToken);
            return await producer.SendAsync(bytes, properties, cancellationToken);
        }

        /// <summary>
        /// Async send message model
        /// 异步发送信息模式
        /// </summary>
        /// <typeparam name="T">General model type</typeparam>
        /// <param name="producer">Producer</param>
        /// <param name="model">Message model</param>
        /// <param name="typeInfo">Model json type info</param>
        /// <param name="properties">Properties</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Message id</returns>
        public static async Task<string> SendJsonAsync<T>(this IMessageQueueProducer producer, T model, JsonTypeInfo<T> typeInfo, MessageProperties? properties = null, CancellationToken cancellationToken = default)
        {
            var bytes = await MessageQueueUtils.ToJsonBytesAsync(model, typeInfo, cancellationToken);
            return await producer.SendAsync(bytes, properties, cancellationToken);
        }

        /// <summary>
        /// Async send message model
        /// 异步发送信息模式
        /// </summary>
        /// <typeparam name="T">General model type</typeparam>
        /// <param name="producer">Producer</param>
        /// <param name="model">Message model</param>
        /// <param name="typeInfo">Model json type info</param>
        /// <param name="messageType">Message type</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Message id</returns>
        public static Task<string> SendJsonAsync<T>(this IMessageQueueProducer producer, T model, JsonTypeInfo<T> typeInfo, string messageType, CancellationToken cancellationToken = default)
        {
            return SendJsonAsync(producer, model, typeInfo, new MessageProperties { Type = messageType }, cancellationToken);
        }
    }
}
