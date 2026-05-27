using com.etsoo.MessageQueue.QueueProcessors;
using com.etsoo.Utils.String;
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
        public const string RetryCountField = "x-retry-count";

        readonly LocalRabbitMQConsumerOptions _options;

        AsyncEventingBasicConsumer? _consumer;

        QueueDeclareOk[] _delayedQueues = [];

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
                    // Get rid of the unknow messages, let them acknowledged
                    // await channel.BasicNackAsync(e.DeliveryTag, false, true, e.CancellationToken);
                    if (Logger.IsEnabled(LogLevel.Warning))
                    {
                        // Log warning
                        Logger.LogError("No Processor for Message: {message}, Properties: {properties}", e.Body.ToJsonString(), properties);
                    }
                }
                else if (count > 1)
                {
                    if (Logger.IsEnabled(LogLevel.Information))
                    {
                        // Log warning
                        Logger.LogInformation("More Than One Processor for Message: {message}, Properties: {properties}", e.Body.ToJsonString(), properties);
                    }
                }

                await channel.BasicAckAsync(e.DeliveryTag, false, e.CancellationToken);

                if (Logger.IsEnabled(LogLevel.Information))
                {
                    // Log success
                    Logger.LogInformation("Message {id} Processed {count}", properties.MessageId, count);
                }
            }
            catch (Exception ex)
            {
                if (Logger.IsEnabled(LogLevel.Error))
                {
                    Logger.LogError(ex, "Message: {message}, Properties: {properties}", e.Body.ToJsonString(), properties);
                }

                // TTL + DLX
                object? retryCountObj = null;
                properties.Headers?.TryGetValue(RetryCountField, out retryCountObj);

                var retryCount = StringUtils.TryParseObject<int>(retryCountObj).GetValueOrDefault() + 1;

                var delayQueue = GetDelayQueue(retryCount);

                var props = new BasicProperties(e.BasicProperties)
                {
                    Persistent = true
                };
                props.Headers ??= new Dictionary<string, object?>();
                props.Headers[RetryCountField] = retryCount;

                await channel.BasicPublishAsync(exchange: "",
                                     routingKey: delayQueue.QueueName,
                                     mandatory: _options.Mandatory,
                                     basicProperties: props,
                                     body: e.Body,
                                     cancellationToken: e.CancellationToken);

                await channel.BasicAckAsync(e.DeliveryTag, false, e.CancellationToken);
                // await channel.BasicNackAsync(e.DeliveryTag, false, true, e.CancellationToken);
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

            var delayQueue1 = await CreateDelayQueue(channel, queue, GetRetryDelay(1), cancellationToken);
            var delayQueue2 = await CreateDelayQueue(channel, queue, GetRetryDelay(2), cancellationToken);
            var delayQueue3 = await CreateDelayQueue(channel, queue, GetRetryDelay(3), cancellationToken);
            var delayQueue4 = await CreateDelayQueue(channel, queue, GetRetryDelay(4), cancellationToken);
            _delayedQueues = [delayQueue1, delayQueue2, delayQueue3, delayQueue4];

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += MessageHandler;

            await channel.BasicConsumeAsync(queue: queue,
                     autoAck: false,
                     consumer: consumer,
                     cancellationToken: cancellationToken);

            _consumer = consumer;
        }

        private QueueDeclareOk GetDelayQueue(int retryCount)
        {
            var index = retryCount - 1;
            if (index >= _delayedQueues.Length)
            {
                return _delayedQueues[^1];
            }
            else
            {
                return _delayedQueues[index];
            }
        }

        private Task<QueueDeclareOk> CreateDelayQueue(IChannel channel, string queue, int seconds, CancellationToken cancellationToken)
        {
            // durable, whether the queue is durable, 'true' means it will survive a broker restart
            // exclusive, whether the queue belongs ONLY to the current connection, 'true' means it's inaccessible from other connections, automatically deleted when the connection closes
            // autoDelete, whether the queue is automatically deleted when the last consumer unsubscribes
            return channel.QueueDeclareAsync(queue: queue + $"-DLX-{seconds}",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: new Dictionary<string, object?>
                {
                    { "x-dead-letter-exchange", string.IsNullOrEmpty(_options.Exchange) ? "" : _options.Exchange },
                    { "x-dead-letter-routing-key", _options.RoutingKey ?? queue },
                    { "x-message-ttl", seconds * 1000 }
                },
                cancellationToken: cancellationToken);
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
