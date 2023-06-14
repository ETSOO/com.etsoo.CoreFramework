using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace com.etsoo.MessageQueue.GooglePubSub
{
    /// <summary>
    /// Google PubSub message consumer extensions
    /// 谷歌 PubSub 消息消费者扩展
    /// </summary>
    public static class GooglePubSubConsumerExtensions
    {
        /// <summary>
        /// Add Google PubSub message consumer to service collection
        /// 将 Google PubSub 消息消费者添加到服务集合
        /// </summary>
        /// <param name="services">Services</param>
        /// <param name="options">Options</param>
        /// <returns>Result</returns>
        public static IServiceCollection AddGooglePubSubConsumer(this IServiceCollection services, GooglePubSubConsumerOptions options)
        {
            services.AddSingleton<IMessageQueueConsumer, GooglePubSubConsumer>((provider) =>
            {
                return new GooglePubSubConsumer(GooglePubSubUtils.CreateSubscriberClient(options), provider.GetServices<IMessageQueueProcessor>(), provider.GetRequiredService<ILogger<GooglePubSubConsumer>>());
            });
            return services;
        }
    }
}
