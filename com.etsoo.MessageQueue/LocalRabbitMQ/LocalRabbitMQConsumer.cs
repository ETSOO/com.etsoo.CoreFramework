using com.etsoo.MessageQueue.QueueProcessors;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace com.etsoo.MessageQueue.LocalRabbitMQ
{
    /// <summary>
    /// Local RabbitMQ message consumer
    /// 本地 RabbitMQ 消息消费者
    /// </summary>
    public class LocalRabbitMQConsumer : MessageQueueConsumer
    {
        readonly LocalRabbitMQConsumerOptions _options;

        AsyncEventingBasicConsumer? _consumer;

        public LocalRabbitMQConsumer(LocalRabbitMQConsumerOptions options, IEnumerable<IMessageQueueProcessor> processors, ILogger logger)
            : base(processors, logger)
        {
            _options = options;
        }

        private async Task MessageHandler(object sender, BasicDeliverEventArgs e)
        {
            if (sender is not AsyncEventingBasicConsumer consumer) return;

            var channel = consumer.Channel;

            var properties = new MessageReceivedProperties();
            try
            {
                properties = LocalRabbitMQUtils.CreatePropertiesFromArgs(e);
                var count = await ProcessAsync(e.Body, properties, e.CancellationToken);
                if (count == 0)
                {
                    await channel.BasicNackAsync(e.DeliveryTag, false, true, e.CancellationToken);

                    // Log warning
                    Logger.LogError("No Processor for Message: {message}, Properties: {properties}", e.Body.ToJsonString(), properties);

                    return;
                }
                else if (count > 1)
                {
                    // Log warning
                    Logger.LogWarning("More Than One Processor for Message: {message}, Properties: {properties}", e.Body.ToJsonString(), properties);
                }

                await channel.BasicAckAsync(e.DeliveryTag, false, e.CancellationToken);

                // Log success
                Logger.LogInformation("Message {id} Processed {count}", properties.MessageId, count);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Message: {message}, Properties: {properties}", e.Body.ToJsonString(), properties);
                await channel.BasicNackAsync(e.DeliveryTag, false, true, e.CancellationToken);
            }
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service
        /// 当程序准备好启动服务时触发
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var connection = await LocalRabbitMQUtils.CreateConnectionAsync(_options, cancellationToken);

            var channel = await connection.CreateChannelAsync(null, cancellationToken);

            var exchange = _options.Exchange;
            string queue;
            if (string.IsNullOrEmpty(exchange))
            {
                var queueName = _options.QueueName;
                if (string.IsNullOrEmpty(queueName)) throw new Exception("Please define QueueName");

                queue = queueName;

                await channel.QueueDeclareAsync(queue: queue,
                                     durable: _options.Durable,
                                     exclusive: _options.Exclusive,
                                     autoDelete: _options.AutoDelete,
                                     arguments: _options.QueueArguments,
                                     cancellationToken: cancellationToken);

                var qos = _options.QosOptions;
                if (qos != null)
                {
                    await channel.BasicQosAsync(
                        prefetchSize: qos.PrefetchSize,
                        prefetchCount: qos.PrefetchCount,
                        global: qos.Global,
                        cancellationToken: cancellationToken);
                }
            }
            else
            {
                var exchangeType = _options.ExchangeType ?? ExchangeType.Fanout;

                var routingKey = _options.RoutingKey;

                if (exchangeType == ExchangeType.Fanout) routingKey = string.Empty;
                else if (string.IsNullOrEmpty(routingKey)) throw new Exception("Please define RoutingKey");

                await channel.ExchangeDeclareAsync(exchange: exchange, type: exchangeType, cancellationToken: cancellationToken);

                queue = (await channel.QueueDeclareAsync(cancellationToken: cancellationToken)).QueueName;

                await channel.QueueBindAsync(queue: queue,
                  exchange: exchange,
                  routingKey: routingKey,
                  arguments: _options.QueueArguments,
                  cancellationToken: cancellationToken);
            }

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += MessageHandler;

            await channel.BasicConsumeAsync(queue: queue,
                     autoAck: false,
                     consumer: consumer,
                     cancellationToken: cancellationToken);

            _consumer = consumer;
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown
        /// 当程序执行正常关闭时触发
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_consumer != null)
            {
                _consumer.ReceivedAsync -= MessageHandler;

                if (_consumer.Channel.IsOpen)
                    await _consumer.Channel.CloseAsync(cancellationToken);

                await _consumer.Channel.DisposeAsync();

                _consumer = null;
            }
        }

        /// <summary>
        /// Dispose of resources
        /// 处置资源
        /// </summary>
        public override void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
