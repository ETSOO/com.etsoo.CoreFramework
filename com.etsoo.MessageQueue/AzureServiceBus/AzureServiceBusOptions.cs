using Azure.Messaging.ServiceBus;
using System.ComponentModel.DataAnnotations;

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
        [Required]
        public string ConnectionString { get; set; } = default!;

        /// <summary>
        /// Client options
        /// 客户端选项
        /// </summary>
        public ServiceBusClientOptions ClientOptions { get; set; } = new();
    }
}
