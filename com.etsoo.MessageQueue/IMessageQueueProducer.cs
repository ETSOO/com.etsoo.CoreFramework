namespace com.etsoo.MessageQueue
{
    /// <summary>
    /// Message queue producer interface
    /// 消息队列生产者接口，如果需要注册多个Topic，使用HttpClient服务类似的方式，每一个服务对应一个生产者
    /// </summary>
    public interface IMessageQueueProducer : IAsyncDisposable
    {
        /// <summary>
        /// Async send message bytes
        /// 异步发送信息字节
        /// </summary>
        /// <param name="body">Message body</param>
        /// <param name="properties">Message properties</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Message id</returns>
        Task<string> SendAsync(ReadOnlyMemory<byte> body, MessageProperties? properties = null, CancellationToken cancellationToken = default);
    }
}
