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
        private readonly LocalRabbitMQConsumerOptions _options;

        public LocalRabbitMQConsumer(LocalRabbitMQConsumerOptions options, IEnumerable<IMessageQueueProcessor> processors, ILogger logger)
            : base(processors, logger)
        {
            _options = options;
        }

        public override async Task ReceiveAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() =>
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
                consumer.Received += async (sender, e) =>
                {
                    var properties = new MessageReceivedProperties();
                    try
                    {
                        properties = LocalRabbitMQUtils.CreatePropertiesFromArgs(e);
                        await ProcessAsync(e.Body, properties, cancellationToken);
                        channel.BasicAck(e.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex, "Message: {message}, Properties: {properties}", e.Body.ToJsonString(), properties);
                        channel.BasicNack(e.DeliveryTag, false, true);
                    }
                };

                channel.BasicConsume(queue: queue,
                         autoAck: false,
                         consumer: consumer);

                // Keep running
                while (!cancellationToken.IsCancellationRequested)
                {
                }

                channel.Dispose();
            }, cancellationToken);
        }
    }
}
