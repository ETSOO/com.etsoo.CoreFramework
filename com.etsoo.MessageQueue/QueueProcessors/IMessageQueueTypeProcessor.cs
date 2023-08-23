namespace com.etsoo.MessageQueue.QueueProcessors
{
    /// <summary>
    /// Message queue processor by type
    /// 消息队列类型处理器
    /// </summary>
    public interface IMessageQueueTypeProcessor
    {
        /// <summary>
        /// Determines whether the specified message can be deserialized, make it as lightweight as possible
        /// 判断指定消息是否可以反序列化，尽可能轻巧
        /// </summary>
        /// <param name="messageType">Message type</param>
        /// <returns>Result</returns>
        public bool CanDeserialize(string messageType);

        /// <summary>
        /// Convert, map and process the message
        /// 转换、映射和处理消息
        /// </summary>
        /// <param name="body">Message body</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public Task ExecuteAsync(ReadOnlyMemory<byte> body, CancellationToken cancellationToken);
    }
}