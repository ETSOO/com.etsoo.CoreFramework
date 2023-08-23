namespace com.etsoo.MessageQueue.QueueProcessors
{
    /// <summary>
    /// Message queue processor
    /// 消息队列处理器
    /// </summary>
    public interface IMessageQueueProcessor
    {
        /// <summary>
        /// Determines whether the specified message with MessageProperties can be deserialized, make it as lightweight as possible
        /// 判断具有 MessageProperties 的指定消息是否可以反序列化，尽可能轻巧
        /// </summary>
        /// <param name="properties">Received message properties</param>
        /// <returns>Result</returns>
        public bool CanDeserialize(MessageReceivedProperties properties);

        /// <summary>
        /// Convert, map and process the message
        /// 转换、映射和处理消息
        /// </summary>
        /// <param name="body">Message body</param>
        /// <param name="properties">Received message properties</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public Task ExecuteAsync(ReadOnlyMemory<byte> body, MessageReceivedProperties properties, CancellationToken cancellationToken);
    }
}
