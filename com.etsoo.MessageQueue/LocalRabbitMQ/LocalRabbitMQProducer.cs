using RabbitMQ.Client;

namespace com.etsoo.MessageQueue.LocalRabbitMQ
{
    /// <summary>
    /// Local RabbitMQ message producer
    /// 本地 RabbitMQ 消息生产者
    /// </summary>
    public class LocalRabbitMQProducer : IMessageQueueProducer
    {
        private readonly LocalRabbitMQProducerOptions _options;

        public LocalRabbitMQProducer(LocalRabbitMQProducerOptions options)
        {
            _options = options;
        }

        public async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            await Task.CompletedTask;
        }

        public async Task<string> SendAsync(ReadOnlyMemory<byte> body, MessageProperties? properties = null, CancellationToken cancellationToken = default)
        {
            var connection = await LocalRabbitMQUtils.CreateConnectionAsync(_options, cancellationToken);

            using var channel = await connection.CreateChannelAsync(new CreateChannelOptions(true, true), cancellationToken);

            var exchange = _options.Exchange;
            var routingKey = _options.RoutingKey;

            if (string.IsNullOrEmpty(exchange))
            {
                // Work Queues. Distributing tasks among workers
                exchange = string.Empty;

                var queueName = _options.QueueName;
                if (string.IsNullOrEmpty(queueName)) throw new Exception("Please define QueueName");

                routingKey = queueName;

                await channel.QueueDeclareAsync(queue: queueName,
                                     durable: _options.Durable,
                                     exclusive: _options.Exclusive,
                                     autoDelete: _options.AutoDelete,
                                     arguments: _options.QueueArguments,
                                     cancellationToken: cancellationToken);
            }
            else
            {
                // Publish/Subscribe (Fanout Exchange). Messages are broadcast to all queues bound to the exchange. The exchange doesn't care about the routing key.
                // Routing (Direct Exchange). Messages are routed to queues based on an exact match of the routing key.
                // Topics (Topic Exchange). Messages are routed to queues based on pattern matching with the routing key
                var exchangeType = _options.ExchangeType ?? ExchangeType.Fanout;

                if (exchangeType == ExchangeType.Fanout) routingKey = string.Empty;
                else if (string.IsNullOrEmpty(routingKey)) throw new Exception("Please define RoutingKey");

                await channel.ExchangeDeclareAsync(exchange: exchange, type: exchangeType, cancellationToken: cancellationToken);
            }

            var bp = new BasicProperties
            {
                Persistent = true
            };

            var messageId = Guid.NewGuid().ToString();
            bp.MessageId = messageId;

            if (properties != null)
            {
                if (properties.AppId != null) bp.AppId = properties.AppId;
                if (properties.CorrelationId != null) bp.CorrelationId = properties.CorrelationId;
                if (properties.Type != null) bp.Type = properties.Type;
                if (properties.ContentEncoding != null) bp.ContentEncoding = properties.ContentEncoding;
                if (properties.ContentType != null) bp.ContentType = properties.ContentType;
                if (properties.Priority.HasValue) bp.Priority = properties.Priority.Value;
                if (properties.ReplyTo != null) bp.ReplyTo = properties.ReplyTo;
                if (properties.TimeToLive.HasValue) bp.Expiration = properties.TimeToLive.Value.TotalMilliseconds.ToString();

                if (properties.Headers != null) bp.Headers = properties.Headers!;
                else bp.Headers ??= new Dictionary<string, object?>();

                if (bp.Headers.TryGetValue(LocalRabbitMQUtils.LoginUserIdField, out var user) && user != null)
                {
                    bp.UserId = Convert.ToString(user);
                }

                if (properties.UserId != null) bp.Headers[nameof(properties.UserId)] = properties.UserId;
            }

            await channel.BasicPublishAsync(exchange: exchange,
                                 routingKey: routingKey,
                                 mandatory: _options.Mandatory,
                                 basicProperties: bp,
                                 body: body,
                                 cancellationToken: cancellationToken);

            return messageId;
        }
    }
}
