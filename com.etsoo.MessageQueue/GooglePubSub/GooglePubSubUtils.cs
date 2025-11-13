using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;

namespace com.etsoo.MessageQueue.GooglePubSub
{
    /// <summary>
    /// Google PubSub utilities
    /// Google PubSub 工具
    /// </summary>
    public static class GooglePubSubUtils
    {
        private static void SetupClientBuilder()
        {

        }

        /// <summary>
        /// Setup publisher client builder
        /// 设置发布者客户端生成器
        /// </summary>
        /// <param name="builder">Builder</param>
        /// <param name="options">Options</param>
        public static void SetupPublisherClientBuilder(PublisherClientBuilder builder, GooglePubSubProducerOptions options)
        {
            if (options.Endpoint != null) builder.Endpoint = options.Endpoint;
            if (options.CredentialsPath != null) builder.Credential = CredentialFactory.FromFile<ServiceAccountCredential>(options.CredentialsPath);
            if (options.JsonCredentials != null) builder.Credential = CredentialFactory.FromJson<ServiceAccountCredential>(options.JsonCredentials);
            if (options.ClientCount.HasValue) builder.ClientCount = options.ClientCount.Value;
            if (options.QuotaProject != null) builder.QuotaProject = options.QuotaProject;
            if (options.UserAgent != null) builder.UserAgent = options.UserAgent;

            builder.TopicName = new TopicName(options.ProjectId, options.TopicId);
        }

        /// <summary>
        /// Setup subscriber client builder
        /// 设置订阅者客户端生成器
        /// </summary>
        /// <param name="builder">Builder</param>
        /// <param name="options">Options</param>
        public static void SetupSubscriberClientBuilder(SubscriberClientBuilder builder, GooglePubSubConsumerOptions options)
        {
            if (options.Endpoint != null) builder.Endpoint = options.Endpoint;
            if (options.CredentialsPath != null) builder.Credential = CredentialFactory.FromFile<ServiceAccountCredential>(options.CredentialsPath);
            if (options.JsonCredentials != null) builder.Credential = CredentialFactory.FromJson<ServiceAccountCredential>(options.JsonCredentials);
            if (options.ClientCount.HasValue) builder.ClientCount = options.ClientCount.Value;
            if (options.QuotaProject != null) builder.QuotaProject = options.QuotaProject;
            if (options.UserAgent != null) builder.UserAgent = options.UserAgent;

            builder.SubscriptionName = new SubscriptionName(options.ProjectId, options.SubscriptionId);
        }

        /// <summary>
        /// Create received message properties
        /// 创建收到的消息属性
        /// </summary>
        /// <param name="message">Received message</param>
        /// <returns>Result</returns>
        public static MessageReceivedProperties CreatePropertiesFromMessage(PubsubMessage message)
        {
            var p = new MessageReceivedProperties
            {
                MessageId = message.MessageId,
                Timestamp = message.PublishTime.Seconds * 1000,
                Headers = message.Attributes.ToDictionary(entry => entry.Key, entry => (object)entry.Value)
            };

            p.CorrelationId = p.GetHeader(nameof(p.CorrelationId));
            p.AppId = p.GetHeader(nameof(p.AppId));
            p.ContentEncoding = p.GetHeader(nameof(p.ContentEncoding));
            p.ContentType = p.GetHeader(nameof(p.ContentType));
            p.Priority = p.GetHeader<byte>(nameof(p.Priority));
            p.ReplyTo = p.GetHeader(nameof(p.ReplyTo));
            p.Type = p.GetHeader(nameof(p.Type));
            p.UserId = p.GetHeader(nameof(p.UserId));

            p.Headers[nameof(message.OrderingKey)] = message.OrderingKey;

            return p;
        }

        /// <summary>
        /// Create publisher client
        /// 创建发布者客户端
        /// </summary>
        /// <param name="options">Options</param>
        /// <returns>Result</returns>
        public static PublisherClient CreatePublisherClient(GooglePubSubProducerOptions options)
        {
            var builder = new PublisherClientBuilder();
            SetupPublisherClientBuilder(builder, options);
            return builder.Build();
        }

        /// <summary>
        /// Async create publisher client
        /// 异步创建发布者客户端
        /// </summary>
        /// <param name="options">Options</param>
        /// <returns>Result</returns>
        public static async Task<PublisherClient> CreatePublisherClientAsync(GooglePubSubProducerOptions options)
        {
            var builder = new PublisherClientBuilder();
            SetupPublisherClientBuilder(builder, options);
            return await builder.BuildAsync();
        }


        /// <summary>
        /// Create subscriber client
        /// 创建订阅客户端
        /// </summary>
        /// <param name="options">Options</param>
        /// <returns>Result</returns>
        public static SubscriberClient CreateSubscriberClient(GooglePubSubConsumerOptions options)
        {
            var builder = new SubscriberClientBuilder();
            SetupSubscriberClientBuilder(builder, options);
            return builder.Build();
        }

        /// <summary>
        /// Async create subscriber client
        /// 异步创建订阅客户端
        /// </summary>
        /// <param name="options">Options</param>
        /// <returns>Result</returns>
        public static async Task<SubscriberClient> CreateSubscriberClientAsync(GooglePubSubConsumerOptions options)
        {
            var builder = new SubscriberClientBuilder();
            SetupSubscriberClientBuilder(builder, options);
            return await builder.BuildAsync();
        }
    }
}
