using Azure.Messaging.ServiceBus;

namespace com.etsoo.MessageQueue.AzureServiceBus
{
    /// <summary>
    /// Azure service bus message producer options
    /// Azure 服务总线消息生成器选项
    /// </summary>
    public record AzureServiceBusProducerOptions : AzureServiceBusOptions
    {
        /// <summary>
        /// Queue or topic name
        /// 队列或主题名称
        /// </summary>
        public required string QueueOrTopicName { get; init; }

        /// <summary>
        /// Sender options
        /// 发件人选项
        /// </summary>
        public ServiceBusSenderOptions SenderOptions { get; init; } = new();
    }
}
