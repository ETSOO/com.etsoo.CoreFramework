using com.etsoo.MessageQueue.QueueProcessors;
using Google.Cloud.PubSub.V1;
using Microsoft.Extensions.Logging;

namespace com.etsoo.MessageQueue.GooglePubSub
{
    /// <summary>
    /// Google PubSub message consumer
    /// Google PubSub 消息消费者
    /// </summary>
    public class GooglePubSubConsumer : MessageQueueConsumer
    {
        private readonly SubscriberClient _subscriberClient;

        public GooglePubSubConsumer(SubscriberClient subscriberClient, IEnumerable<IMessageQueueProcessor> processors, ILogger logger)
            : base(processors, logger)
        {
            _subscriberClient = subscriberClient;
        }

        public override async Task ReceiveAsync(CancellationToken cancellationToken)
        {
            var startTask = _subscriberClient.StartAsync(async (message, cancel) =>
            {
                var properties = new MessageReceivedProperties();
                try
                {
                    properties = GooglePubSubUtils.CreatePropertiesFromMessage(message);
                    await ProcessAsync(message.Data.ToByteArray(), properties, cancel);
                    return SubscriberClient.Reply.Ack;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Message: {message}, Properties: {properties}", message.Data.ToStringUtf8(), properties);
                    return SubscriberClient.Reply.Nack;
                }
            });

            // Keep running
            while (!cancellationToken.IsCancellationRequested)
            {
            }

            // Stop
            await _subscriberClient.StopAsync(CancellationToken.None);

            // Lets make sure that the start task finished successfully after the call to stop
            await startTask;
        }
    }
}
