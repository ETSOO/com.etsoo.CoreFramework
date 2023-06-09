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
        /// <param name="model">Message model</param>
        /// <param name="properties">Properties</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Message id</returns>
        public static async Task<string> SendJsonAsync<T>(this IMessageQueueProducer producer, T model, MessageProperties? properties = null, CancellationToken cancellationToken = default)
        {
            var bytes = await MessageQueueUtils.ToJsonBytesAsync(model, cancellationToken);
            return await producer.SendAsync(bytes, properties, cancellationToken);
        }
    }
}
