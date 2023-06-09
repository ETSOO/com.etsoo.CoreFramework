using Microsoft.Extensions.DependencyInjection;

namespace com.etsoo.MessageQueue.GooglePubSub
{
    /// <summary>
    /// Google PubSub message producer extensions
    /// 谷歌 PubSub 消息生产者扩展
    /// </summary>
    public static class GooglePubSubProducerExtensions
    {
        /// <summary>
        /// Add Google PubSub message producer to service collection
        /// 将谷歌 PubSub 消息生产者添加到服务集合
        /// </summary>
        /// <param name="services">Services</param>
        /// <param name="options">Options</param>
        /// <returns>Result</returns>
        public static IServiceCollection AddGooglePubSubProducer(this IServiceCollection services, GooglePubSubProducerOptions options)
        {
            var producer = new GooglePubSubProducer(GooglePubSubUtils.CreatePublisherClient(options));
            services.AddSingleton<IMessageQueueProducer>(producer);
            return services;
        }
    }
}
