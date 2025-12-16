namespace com.etsoo.MessageQueue.QueueProcessors
{
    /// <summary>
    /// Dynamic queue processor
    /// 动态队列处理器
    /// </summary>
    public abstract class DynamicQueueProcessor : IMessageQueueProcessor
    {
        /// <summary>
        /// Type to check
        /// </summary>
        public abstract string Type { get; }

        /// <summary>
        /// Can deserialize message or not
        /// 是否能反序列化消息
        /// </summary>
        /// <param name="properties">Message properties</param>
        /// <returns>Result</returns>
        public virtual bool CanDeserialize(MessageReceivedProperties properties)
        {
            // Check type
            return properties.Type?.Equals(Type) is true;
        }

        /// <summary>
        /// Execute
        /// 执行
        /// </summary>
        /// <param name="body">Message body</param>
        /// <param name="properties">Message properties</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        public abstract Task ExecuteAsync(ReadOnlyMemory<byte> body, MessageReceivedProperties properties, CancellationToken cancellationToken);
    }
}
