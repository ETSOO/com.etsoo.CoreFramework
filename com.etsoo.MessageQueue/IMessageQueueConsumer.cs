namespace com.etsoo.MessageQueue
{
    /// <summary>
    /// Message queue consumer interface
    /// 消息队列消费者接口，如果需要注册多个Topic，使用HttpClient服务类似的方式，每一个服务对应一个消费者
    /// </summary>
    public interface IMessageQueueConsumer
    {
        /// <summary>
        /// Async receive messages
        /// 异步接收消息
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        Task ReceiveAsync(CancellationToken cancellationToken);
    }
}
