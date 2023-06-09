using Microsoft.Extensions.DependencyInjection;

namespace com.etsoo.MessageQueue.AzureServiceBus
{
    /// <summary>
    /// Azure service bus message producer extensions
    /// Azure 服务总线消息生成器扩展
    /// </summary>
    public static class AzureServiceBusProducerExtensions
    {
        /// <summary>
        /// Add Azure message producer to service collection
        /// 将 Azure 消息生产者添加到服务集合
        /// </summary>
        /// <param name="services">Services</param>
        /// <param name="options">Options</param>
        /// <returns>Result</returns>
        public static IServiceCollection AddAzureServiceBusProducer(this IServiceCollection services, AzureServiceBusProducerOptions options)
        {
            var producer = new AzureServiceBusProducer(AzureServiceBusUtils.CreateServiceBusSender(options));
            services.AddSingleton<IMessageQueueProducer>(producer);
            return services;
        }
    }
}
