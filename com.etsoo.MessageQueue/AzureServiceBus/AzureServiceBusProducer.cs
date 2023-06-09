using Azure.Messaging.ServiceBus;

namespace com.etsoo.MessageQueue.AzureServiceBus
{
    /// <summary>
    /// Azure service bus message producer
    /// Azure 服务总线消息生成器
    /// </summary>
    public class AzureServiceBusProducer : IMessageQueueProducer
    {
        private readonly ServiceBusSender _sender;

        public AzureServiceBusProducer(ServiceBusSender sender)
        {
            _sender = sender;
        }

        public async ValueTask DisposeAsync()
        {
            await _sender.DisposeAsync();
            GC.SuppressFinalize(this);
        }

        public async Task<string> SendAsync(ReadOnlyMemory<byte> body, MessageProperties? properties = null, CancellationToken cancellationToken = default)
        {
            var messageId = Guid.NewGuid().ToString();

            var message = new ServiceBusMessage(body)
            {
                MessageId = messageId,
            };

            if (properties != null)
            {
                if (properties.CorrelationId != null) message.CorrelationId = properties.CorrelationId;

                if (properties.AppId != null) message.ApplicationProperties.Add(nameof(properties.AppId), properties.AppId);
                if (properties.Type != null) message.ApplicationProperties.Add(nameof(properties.Type), properties.Type);
                if (properties.UserId != null) message.ApplicationProperties.Add(nameof(properties.UserId), properties.UserId);
                if (properties.ContentEncoding != null) message.ApplicationProperties.Add(nameof(properties.ContentEncoding), properties.ContentEncoding);
                if (properties.ContentType != null) message.ContentType = properties.ContentType;
                if (properties.Priority.HasValue) message.ApplicationProperties.Add(nameof(properties.Priority), properties.Priority.Value);
                if (properties.ReplyTo != null) message.ReplyTo = properties.ReplyTo;
                if (properties.TimeToLive.HasValue) message.TimeToLive = properties.TimeToLive.Value;
                if (properties.Headers != null)
                {
                    foreach (var header in properties.Headers)
                    {
                        message.ApplicationProperties[header.Key] = header.Value;
                    }
                }
            }

            await _sender.SendMessageAsync(message, cancellationToken);

            return messageId;
        }
    }
}
