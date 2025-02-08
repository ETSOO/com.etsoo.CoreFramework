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
        /// Process message
        /// 处理消息
        /// </summary>
        /// <param name="body">Message body</param>
        /// <param name="properties">Message properties</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Execution count</returns>
        /// <exception cref="AggregateException">Aggregate exception</exception>
        protected async Task<int> ProcessAsync(ReadOnlyMemory<byte> body, MessageReceivedProperties properties, CancellationToken cancellationToken)
        {
            var exceptions = new ConcurrentQueue<Exception>();
            var count = 0;

            await Parallel.ForEachAsync(_processors, cancellationToken, async (processor, cancellationToken) =>
            {
                try
                {
                    // Quick check
                    if (!processor.CanDeserialize(properties)) return;

                    // Convert, map and process
                    await processor.ExecuteAsync(body, properties, cancellationToken);

                    // Increase execution count
                    Interlocked.Increment(ref count);
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

            return count;
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service
        /// 当程序准备好启动服务时触发
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public abstract Task StartAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown
        /// 当程序执行正常关闭时触发
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public abstract Task StopAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Dispose of resources
        /// 处置资源
        /// </summary>
        public abstract void Dispose();
    }
}
