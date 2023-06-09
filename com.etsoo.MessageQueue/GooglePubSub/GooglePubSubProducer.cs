using Google.Cloud.PubSub.V1;
using Google.Protobuf;

namespace com.etsoo.MessageQueue.GooglePubSub
{
    /// <summary>
    /// Google PubSub message producer
    /// Google PubSub 消息生产者
    /// </summary>
    public class GooglePubSubProducer : IMessageQueueProducer
    {
        private readonly PublisherClient _publisherClient;

        public GooglePubSubProducer(PublisherClient publisherClient)
        {
            _publisherClient = publisherClient;
        }

        public async ValueTask DisposeAsync()
        {
            await _publisherClient.DisposeAsync();
            GC.SuppressFinalize(this);
        }

        public async Task<string> SendAsync(ReadOnlyMemory<byte> body, MessageProperties? properties = null, CancellationToken cancellationToken = default)
        {
            var data = ByteString.CopyFrom(body.Span);

            var message = new PubsubMessage
            {
                Data = data
            };

            if (properties != null)
            {
                if (properties.AppId != null) message.Attributes.Add(nameof(properties.AppId), properties.AppId);
                if (properties.CorrelationId != null) message.Attributes.Add(nameof(properties.CorrelationId), properties.CorrelationId);
                if (properties.Type != null) message.Attributes.Add(nameof(properties.Type), properties.Type);
                if (properties.UserId != null) message.Attributes.Add(nameof(properties.UserId), properties.UserId);
                if (properties.ContentEncoding != null) message.Attributes.Add(nameof(properties.ContentEncoding), properties.ContentEncoding);
                if (properties.ContentType != null) message.Attributes.Add(nameof(properties.ContentType), properties.ContentType);
                if (properties.Priority.HasValue) message.Attributes.Add(nameof(properties.Priority), properties.Priority.Value.ToString());
                if (properties.ReplyTo != null) message.Attributes.Add(nameof(properties.ReplyTo), properties.ReplyTo);
                if (properties.TimeToLive.HasValue) message.Attributes.Add(nameof(properties.TimeToLive), properties.TimeToLive.Value.TotalMilliseconds.ToString());
                if (properties.Headers != null)
                {
                    foreach (var header in properties.Headers)
                    {
                        message.Attributes[header.Key] = Convert.ToString(header.Value);
                    }
                }
            }

            return await _publisherClient.PublishAsync(message);
        }
    }
}
