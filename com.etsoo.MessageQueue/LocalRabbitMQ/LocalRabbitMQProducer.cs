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
            return await Task.Run(() =>
            {
                var connection = LocalRabbitMQUtils.CreateConnection(_options);

                using var channel = connection.CreateModel();

                var exchange = _options.Exchange;
                var routingKey = _options.RoutingKey;

                if (string.IsNullOrEmpty(exchange))
                {
                    exchange = string.Empty;

                    var queueName = _options.QueueName;
                    if (string.IsNullOrEmpty(queueName)) throw new Exception("Please define QueueName");

                    routingKey = queueName;

                    channel.QueueDeclare(queue: queueName,
                                         durable: _options.Durable,
                                         exclusive: _options.Exclusive,
                                         autoDelete: _options.AutoDelete,
                                         arguments: _options.QueueArguments);
                }
                else
                {
                    var exchangeType = _options.ExchangeType ?? ExchangeType.Fanout;

                    if (exchangeType == ExchangeType.Fanout) routingKey = string.Empty;
                    else if (string.IsNullOrEmpty(routingKey)) throw new Exception("Please define RoutingKey");

                    channel.ExchangeDeclare(exchange: exchange, type: exchangeType);
                }

                var bp = channel.CreateBasicProperties();
                bp.Persistent = true;

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

                    if (properties.Headers != null) bp.Headers = properties.Headers;
                    if (bp.Headers.TryGetValue("LoginUserId", out var user))
                    {
                        bp.UserId = Convert.ToString(user);
                    }

                    if (properties.UserId != null) bp.Headers[nameof(properties.UserId)] = properties.UserId;
                }

                channel.ConfirmSelect();

                channel.BasicPublish(exchange: exchange,
                                     routingKey: routingKey,
                                     mandatory: _options.Mandatory,
                                     basicProperties: bp,
                                     body: body);

                channel.WaitForConfirmsOrDie();

                return messageId;
            }, cancellationToken);
        }
    }
}
