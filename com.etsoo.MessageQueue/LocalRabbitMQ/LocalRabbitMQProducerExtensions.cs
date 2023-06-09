using Microsoft.Extensions.DependencyInjection;

namespace com.etsoo.MessageQueue.LocalRabbitMQ
{
    /// <summary>
    /// Local RabbitMQ message producer extensions
    /// 本地 RabbitMQ 消息生产者扩展
    /// </summary>
    public static class LocalRabbitMQProducerExtensions
    {
        /// <summary>
        /// Add local RabbitMQ message producer to service collection
        /// 将本地 RabbitMQ 消息生产者添加到服务集合
        /// </summary>
        /// <param name="services">Services</param>
        /// <param name="options">Options</param>
        /// <returns>Result</returns>
        public static IServiceCollection AddLocalRabbitMQProducer(this IServiceCollection services, LocalRabbitMQProducerOptions options)
        {
            var producer = new LocalRabbitMQProducer(options);
            services.AddSingleton<IMessageQueueProducer>(producer);
            return services;
        }
    }
}
