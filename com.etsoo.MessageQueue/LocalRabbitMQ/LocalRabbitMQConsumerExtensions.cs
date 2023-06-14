using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace com.etsoo.MessageQueue.LocalRabbitMQ
{
    /// <summary>
    /// Local RabbitMQ message consumer extensions
    /// 本地 RabbitMQ 消息消费者扩展
    /// </summary>
    public static class LocalRabbitMQConsumerExtensions
    {
        /// <summary>
        /// Add local RabbitMQ message consumer to service collection
        /// 将本地 RabbitMQ 消息消费者添加到服务集合
        /// </summary>
        /// <param name="services">Services</param>
        /// <param name="options">Options</param>
        /// <returns>Result</returns>
        public static IServiceCollection AddLocalRabbitMQConsumer(this IServiceCollection services, LocalRabbitMQConsumerOptions options)
        {
            services.AddSingleton<IMessageQueueConsumer, LocalRabbitMQConsumer>((provider) =>
            {
                return new LocalRabbitMQConsumer(options, provider.GetServices<IMessageQueueProcessor>(), provider.GetRequiredService<ILogger<LocalRabbitMQConsumer>>());
            });
            return services;
        }
    }
}
