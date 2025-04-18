﻿using com.etsoo.Utils.Serialization;
using Microsoft.Extensions.Logging;
using System.Text.Json.Serialization.Metadata;

namespace com.etsoo.MessageQueue.QueueProcessors
{
    /// <summary>
    /// Common queue processor with type
    /// 通用队列类型处理器
    /// </summary>
    /// <typeparam name="T">Generic message type</typeparam>
    /// <remarks>
    /// Constructor
    /// 构造函数
    /// </remarks>
    /// <param name="logger">Logger</param>
    /// <param name="typeInfo">Json type info</param>
    public abstract class CommonQueueTypeProcessor<T>(ILogger logger, JsonTypeInfo<T> typeInfo) : IMessageQueueTypeProcessor where T : class, IMessageQueueMessage
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
        /// <param name="body">Source body</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        protected abstract Task ProcessMessageAsync(T message, ReadOnlyMemory<byte> body, CancellationToken cancellationToken);

        /// <summary>
        /// Determines whether the specified message can be deserialized, make it as lightweight as possible
        /// 判断指定消息是否可以反序列化，尽可能轻巧
        /// </summary>
        /// <param name="messageType">Message type</param>
        /// <returns>Result</returns>
        public virtual bool CanDeserialize(string messageType)
        {
            return messageType.Equals(T.Type);
        }

        /// <summary>
        /// Convert, map and process the message
        /// 转换、映射和处理消息
        /// </summary>
        /// <param name="body">Message body</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public virtual async Task ExecuteAsync(ReadOnlyMemory<byte> body, CancellationToken cancellationToken)
        {
            var message = await body.ToMessageAsync(typeInfo, cancellationToken);
            if (message == null)
            {
                logger.LogError("Convert body to {type} failed: {body}", typeof(T), body.ToJsonString());
                return;
            }

            // Process
            await ProcessMessageAsync(message, body, cancellationToken);
        }
    }
}
