using Azure.Messaging.ServiceBus;
using com.etsoo.MessageQueue.AzureServiceBus;

namespace com.etsoo.MessageQueue.GooglePubSub
{
    /// <summary>
    /// Azure service bus message consumer options
    /// Azure 服务总线消息消费者选项
    /// </summary>
    public record AzureServiceBusConsumerOptions : AzureServiceBusOptions
    {
        /// <summary>
        /// Queue name
        /// 队列名称
        /// </summary>
        public string? QueueName { get; set; }

        /// <summary>
        /// Topic name
        /// 主题名称
        /// </summary>
        public string? TopicName { get; set; }

        /// <summary>
        /// Subscription name
        /// 订阅名称
        /// </summary>
        public string? SubscriptionName { get; set; }

        /// <summary>
        /// Processor options
        /// 处理器选项
        /// </summary>
        public ServiceBusProcessorOptions ProcessorOptions { get; set; } = new();
    }
}
