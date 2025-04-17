using com.etsoo.Utils.Serialization;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization.Metadata;

namespace com.etsoo.MessageQueue.QueueProcessors
{
    /// <summary>
    /// Common queue processor
    /// 通用队列处理器
    /// </summary>
    /// <typeparam name="T">Generic message type</typeparam>
    /// <remarks>
    /// Constructor
    /// 构造函数
    /// </remarks>
    /// <param name="logger">Logger</param>
    /// <param name="typeInfo">Json type info</param>
    public abstract class CommonQueueProcessor<T>(ILogger logger, JsonTypeInfo<T> typeInfo) : IMessageQueueProcessor where T : class, IMessageQueueMessage
    {
        /// <summary>
        /// Logger
        /// 日志器
        /// </summary>
        protected readonly ILogger logger = logger;

        /// <summary>
        /// Json type info
        /// JSON 类型信息
        /// </summary>
        protected readonly JsonTypeInfo<T> typeInfo = typeInfo;

        /// <summary>
        /// Async process message
        /// 异步处理消息
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="properties">Received message properties</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        protected abstract Task ProcessMessageAsync(T message, MessageReceivedProperties properties, CancellationToken cancellationToken);

        /// <summary>
        /// Determines whether the specified message with MessageProperties can be deserialized, make it as lightweight as possible
        /// 判断具有 MessageProperties 的指定消息是否可以反序列化，尽可能轻巧
        /// </summary>
        /// <param name="properties">Received message properties</param>
        /// <returns>Result</returns>
        public virtual bool CanDeserialize(MessageReceivedProperties properties)
        {
            // Check type
            return properties.Type?.Equals(T.Type) is true;
        }

        /// <summary>
        /// Parse message
        /// 解析消息
        /// </summary>
        /// <param name="body">Message body</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        protected async Task<T?> ParseMessageAsync(ReadOnlyMemory<byte> body, CancellationToken cancellationToken)
        {
            // Mesage
            var message = await body.ToMessageAsync(typeInfo, cancellationToken);
            if (message == null)
            {
                logger.LogError("Convert body to {type} failed with NULL: {body}", typeof(T), body.ToJsonString());
            }

            return message;
        }

        /// <summary>
        /// Convert, map and process the message
        /// 转换、映射和处理消息
        /// </summary>
        /// <param name="body">Message body</param>
        /// <param name="properties">Received message properties</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public virtual async Task ExecuteAsync(ReadOnlyMemory<byte> body, MessageReceivedProperties properties, CancellationToken cancellationToken)
        {
            // Parse message
            var message = await ParseMessageAsync(body, cancellationToken);
            if (message == null)
            {
                // Ignore the message
                return;
            }

            // Process
            await ProcessMessageAsync(message, properties, cancellationToken);
        }
    }
}
