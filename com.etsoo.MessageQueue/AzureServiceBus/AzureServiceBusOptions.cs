using Azure.Messaging.ServiceBus;

namespace com.etsoo.MessageQueue.AzureServiceBus
{
    /// <summary>
    /// Azure service bus options
    /// Azure 服务总线选项
    /// </summary>
    public record AzureServiceBusOptions
    {
        /// <summary>
        /// 连接字符串
        /// Connection string
        /// </summary>
        public required string ConnectionString { get; init; }

        /// <summary>
        /// Client options
        /// 客户端选项
        /// </summary>
        public ServiceBusClientOptions ClientOptions { get; init; } = new();
    }
}
