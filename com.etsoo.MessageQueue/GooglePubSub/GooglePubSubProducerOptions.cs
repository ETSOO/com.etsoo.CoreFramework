namespace com.etsoo.MessageQueue.GooglePubSub
{
    /// <summary>
    /// Google PubSub message producer options
    /// Google PubSub 消息生产者选项
    /// </summary>
    public record GooglePubSubProducerOptions : GooglePubSubOptions
    {
        /// <summary>
        /// Project id
        /// 项目编号
        /// </summary>
        public required string ProjectId { get; init; }

        /// <summary>
        /// Topic id
        /// 主题编号
        /// </summary>
        public required string TopicId { get; init; }
    }
}
