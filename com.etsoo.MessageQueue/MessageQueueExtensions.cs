using Microsoft.Extensions.DependencyInjection;

namespace com.etsoo.MessageQueue
{
    /// <summary>
    /// Message queue extensions
    /// 消息队列扩展
    /// </summary>
    public static class MessageQueueExtensions
    {
        /// <summary>
        /// Add message queue processor to service collection
        /// 将消息队列处理器添加到服务集合
        /// </summary>
        /// <param name="services">Services</param>
        /// <param name="options">Options</param>
        /// <returns>Result</returns>
        public static IServiceCollection AddMessageQueueProcessor<T>(this IServiceCollection services) where T : class, IMessageQueueProcessor
        {
            services.AddSingleton<IMessageQueueProcessor, T>();
            return services;
        }
    }
}
