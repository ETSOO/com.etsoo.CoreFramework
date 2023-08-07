using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace com.etsoo.MessageQueue
{
    /// <summary>
    /// Message queue consumer background worker boilerplate
    /// 消息队列消费者后台工作程序模板
    /// Add processors with services.AddSingleton<IMessageQueueProcessor, ...>();
    /// </summary>
    public class MessageQueueConsumerWorker : BackgroundService
    {
        private readonly ILogger<MessageQueueConsumerWorker> _logger;
        private readonly IMessageQueueConsumer _consumer;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="consumer">Message consumer</param>
        public MessageQueueConsumerWorker(ILogger<MessageQueueConsumerWorker> logger, IMessageQueueConsumer consumer)
        {
            _logger = logger;
            _consumer = consumer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Message queue consumer worker started");

            await _consumer.ReceiveAsync(stoppingToken);

            _logger.LogInformation("Message queue consumer worker completed");
        }
    }
}
