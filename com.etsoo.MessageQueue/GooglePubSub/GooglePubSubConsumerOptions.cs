namespace com.etsoo.MessageQueue.GooglePubSub
{
    /// <summary>
    /// Google PubSub message consumer options
    /// Google PubSub 消息消费者选项
    /// </summary>
    public record GooglePubSubConsumerOptions : GooglePubSubOptions
    {
        /// <summary>
        /// Project id
        /// 项目编号
        /// </summary>
        public required string ProjectId { get; init; }

        /// <summary>
        /// Subscription id
        /// 订阅编号
        /// </summary>
        public required string SubscriptionId { get; init; }
    }
}
