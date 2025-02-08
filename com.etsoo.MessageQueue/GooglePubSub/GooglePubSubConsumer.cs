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

        /// <summary>
        /// Triggered when the application host is ready to start the service
        /// 当程序准备好启动服务时触发
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await _subscriberClient.StartAsync(async (message, cancel) =>
            {
                var properties = new MessageReceivedProperties();
                // using var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancel);
                try
                {
                    properties = GooglePubSubUtils.CreatePropertiesFromMessage(message);
                    var count = await ProcessAsync(message.Data.ToByteArray(), properties, cancel);

                    if (count == 0)
                    {
                        Logger.LogError("No Processor for Message: {message}, Properties: {properties}", message.Data.ToStringUtf8(), properties);
                        return SubscriberClient.Reply.Nack;
                    }
                    else if (count > 1)
                    {
                        Logger.LogWarning("More Than One Processor for Message: {message}, Properties: {properties}", message.Data.ToStringUtf8(), properties);
                    }

                    // Log success
                    Logger.LogInformation("Message {id} Processed {count}", properties.MessageId, count);

                    return SubscriberClient.Reply.Ack;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Message: {message}, Properties: {properties}", message.Data.ToStringUtf8(), properties);
                    return SubscriberClient.Reply.Nack;
                }
            });
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown
        /// 当程序执行正常关闭时触发
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _subscriberClient.StopAsync(cancellationToken);
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
