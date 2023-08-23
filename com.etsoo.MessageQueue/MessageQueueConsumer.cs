using com.etsoo.MessageQueue.QueueProcessors;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace com.etsoo.MessageQueue
{
    /// <summary>
    /// Message queue consumer abstract
    /// 消息队列消费者抽象类
    /// </summary>
    public abstract class MessageQueueConsumer : IMessageQueueConsumer
    {
        private readonly IEnumerable<IMessageQueueProcessor> _processors;

        /// <summary>
        /// Logger
        /// 日志记录器
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="processors">Message processors</param>
        /// <param name="logger">Logger</param>
        public MessageQueueConsumer(IEnumerable<IMessageQueueProcessor> processors, ILogger logger)
        {
            _processors = processors;
            Logger = logger;
        }

        /// <summary>
        /// Async start receive messages
        /// 异步开始接收消息
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        public abstract Task ReceiveAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Process message
        /// 处理消息
        /// </summary>
        /// <param name="body">Message body</param>
        /// <param name="properties">Message properties</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Task</returns>
        /// <exception cref="AggregateException">Aggregate exception</exception>
        protected async Task ProcessAsync(ReadOnlyMemory<byte> body, MessageReceivedProperties properties, CancellationToken cancellationToken)
        {
            var exceptions = new ConcurrentQueue<Exception>();

            await Parallel.ForEachAsync(_processors, cancellationToken, async (processor, cancellationToken) =>
            {
                try
                {
                    // Quick check
                    if (!processor.CanDeserialize(properties)) return;

                    // Convert, map and process
                    await processor.ExecuteAsync(body, properties, cancellationToken);
                }
                catch (Exception ex)
                {
                    exceptions.Enqueue(ex);
                }
            });

            if (!exceptions.IsEmpty)
            {
                throw new AggregateException(exceptions);
            }
        }
    }
}
