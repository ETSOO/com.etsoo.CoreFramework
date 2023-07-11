namespace com.etsoo.MessageQueue
{
    /// <summary>
    /// Message queue message interface
    /// 消息队列消息接口
    /// </summary>
    public interface IMessageQueueMessage
    {
        /// <summary>
        /// Message type
        /// 消息类型
        /// </summary>
        public abstract static string Type { get; }
    }
}