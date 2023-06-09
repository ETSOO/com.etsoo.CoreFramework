using com.etsoo.MessageQueue.GooglePubSub;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace com.etsoo.MessageQueue.AzureServiceBus
{
    /// <summary>
    /// Azure service bus message consumer extensions
    /// Azure 服务总线消息消费者扩展
    /// </summary>
    public static class AzureServiceBusConsumerExtensions
    {
        /// <summary>
        /// Add Azure service bus message consumer to service collection
        /// 将 Azure 服务总线消息消费者添加到服务集合
        /// </summary>
        /// <param name="services">Services</param>
        /// <param name="options">Options</param>
        /// <returns>Result</returns>
        public static IServiceCollection AddAzureServiceBusConsumer(this IServiceCollection services, AzureServiceBusConsumerOptions options)
        {
            services.AddSingleton<IMessageQueueConsumer, AzureServiceBusConsumer>((provider) =>
            {
                return new AzureServiceBusConsumer(AzureServiceBusUtils.CreateServiceBusProcessor(options), provider.GetServices<IMessageQueueProcessor>(), provider.GetRequiredService<ILogger>());
            });
            return services;
        }
    }
}
