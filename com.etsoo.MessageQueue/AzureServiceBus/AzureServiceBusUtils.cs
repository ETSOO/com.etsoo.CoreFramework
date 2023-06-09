using Azure.Messaging.ServiceBus;
using com.etsoo.MessageQueue.GooglePubSub;

namespace com.etsoo.MessageQueue.AzureServiceBus
{
    /// <summary>
    /// Azure service bus utilities
    /// Azure 服务总线工具
    /// </summary>
    public static class AzureServiceBusUtils
    {
        /// <summary>
        /// Create service bus client
        /// 创建服务总线客户端
        /// </summary>
        /// <param name="options">Options</param>
        /// <returns>Result</returns>
        public static ServiceBusClient CreateServiceBusClient(AzureServiceBusOptions options)
        {
            return new ServiceBusClient(options.ConnectionString, options.ClientOptions);
        }

        /// <summary>
        /// Create received message properties
        /// 创建收到的消息属性
        /// </summary>
        /// <param name="message">Received message</param>
        /// <returns>Result</returns>
        public static MessageReceivedProperties CreatePropertiesFromMessage(ServiceBusReceivedMessage message)
        {
            var p = new MessageReceivedProperties
            {
                MessageId = message.MessageId,
                CorrelationId = message.CorrelationId,
                ContentType = message.ContentType,
                ReplyTo = message.ReplyTo,
                Headers = message.ApplicationProperties.ToDictionary(item => item.Key, item => item.Value)
            };

            p.AppId = p.GetHeader(nameof(p.AppId));
            p.ContentEncoding = p.GetHeader(nameof(p.ContentEncoding));
            p.Priority = p.GetHeader<byte>(nameof(p.Priority));
            p.Type = p.GetHeader(nameof(p.Type));
            p.UserId = p.GetHeader(nameof(p.UserId));

            p.Headers[nameof(message.Subject)] = message.Subject;
            p.Headers[nameof(message.SequenceNumber)] = message.SequenceNumber;

            return p;
        }

        /// <summary>
        /// Create service bus processor
        /// 创建服务总线处理器
        /// </summary>
        /// <param name="options">Options</param>
        /// <returns>Result</returns>
        public static ServiceBusProcessor CreateServiceBusProcessor(AzureServiceBusConsumerOptions options)
        {
            var client = CreateServiceBusClient(options);

            if (!string.IsNullOrEmpty(options.QueueName))
            {
                return client.CreateProcessor(options.QueueName, options.ProcessorOptions);
            }
            else if (string.IsNullOrEmpty(options.TopicName))
            {
                throw new Exception("Please define topic name");
            }
            else if (string.IsNullOrEmpty(options.SubscriptionName))
            {
                throw new Exception("Please define subscription name");
            }
            else
            {
                return client.CreateProcessor(options.TopicName, options.SubscriptionName, options.ProcessorOptions);
            }
        }

        /// <summary>
        /// Create service bus sender
        /// 创建服务总线发件人
        /// </summary>
        /// <param name="options">Options</param>
        /// <returns>Result</returns>
        public static ServiceBusSender CreateServiceBusSender(AzureServiceBusProducerOptions options)
        {
            var client = CreateServiceBusClient(options);
            return client.CreateSender(options.QueueOrTopicName, options.SenderOptions);
        }
    }
}
