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
            var consumer = sender as AsyncEventingBasicConsumer;
            if (consumer == null) return;
            var channel = consumer.Model;

            using var cancellationTokenSource = new CancellationTokenSource();

            var properties = new MessageReceivedProperties();
            try
            {
                properties = LocalRabbitMQUtils.CreatePropertiesFromArgs(e);
                await ProcessAsync(e.Body, properties, cancellationTokenSource.Token);
                channel.BasicAck(e.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                cancellationTokenSource.Cancel();

                Logger.LogError(ex, "Message: {message}, Properties: {properties}", e.Body.ToJsonString(), properties);
                channel.BasicNack(e.DeliveryTag, false, true);
            }

            cancellationTokenSource.Dispose();
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service
        /// 当程序准备好启动服务时触发
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var consumer = await Task.Run(() =>
            {
                var connection = LocalRabbitMQUtils.CreateConnection(_options);

                var channel = connection.CreateModel();

                var exchange = _options.Exchange;
                string queue;
                if (string.IsNullOrEmpty(exchange))
                {
                    var queueName = _options.QueueName;
                    if (string.IsNullOrEmpty(queueName)) throw new Exception("Please define QueueName");

                    queue = queueName;

                    channel.QueueDeclare(queue: queue,
                                         durable: _options.Durable,
                                         exclusive: _options.Exclusive,
                                         autoDelete: _options.AutoDelete,
                                         arguments: _options.QueueArguments);

                    var qos = _options.QosOptions;
                    if (qos != null)
                    {
                        channel.BasicQos(prefetchSize: qos.PrefetchSize, prefetchCount: qos.PrefetchCount, global: qos.Global);
                    }
                }
                else
                {
                    var exchangeType = _options.ExchangeType ?? ExchangeType.Fanout;

                    var routingKey = _options.RoutingKey;

                    if (exchangeType == ExchangeType.Fanout) routingKey = string.Empty;
                    else if (string.IsNullOrEmpty(routingKey)) throw new Exception("Please define RoutingKey");

                    channel.ExchangeDeclare(exchange: exchange, type: exchangeType);

                    queue = channel.QueueDeclare().QueueName;

                    channel.QueueBind(queue: queue,
                      exchange: exchange,
                      routingKey: routingKey,
                      arguments: _options.QueueArguments);
                }

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.Received += MessageHandler;

                channel.BasicConsume(queue: queue,
                         autoAck: false,
                         consumer: consumer);

                return consumer;
            }, cancellationToken);

            _consumer = consumer;
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown
        /// 当程序执行正常关闭时触发
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                if (_consumer != null)
                {
                    _consumer.Received -= MessageHandler;

                    if (_consumer.Model.IsOpen)
                        _consumer.Model.Close();

                    _consumer.Model.Dispose();

                    _consumer = null;
                }
            }, cancellationToken);
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
