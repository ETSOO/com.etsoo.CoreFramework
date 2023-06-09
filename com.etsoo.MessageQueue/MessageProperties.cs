namespace com.etsoo.MessageQueue
{
    /// <summary>
    /// Message properties
    /// 消息属性
    /// </summary>
    public record MessageProperties
    {
        /// <summary>
        /// Application Id.
        /// </summary>
        public string? AppId { get; set; }

        /// <summary>
        /// Correlation id
        /// </summary>
        public string? CorrelationId { get; set; }

        /// <summary>
        /// MIME content encoding.
        /// </summary>
        public string? ContentEncoding { get; set; }

        /// <summary>
        /// Content type descriptor
        /// 内容类型描述符
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// Message priority, 0 to 9.
        /// </summary>
        public byte? Priority { get; set; }

        /// <summary>
        /// Destination to reply to.
        /// </summary>
        public string? ReplyTo { get; set; }

        /// <summary>
        /// Message type name.
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// User Id.
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// The message’s "time to live (TTL)" value
        /// </summary>
        public TimeSpan? TimeToLive { get; set; }

        /// <summary>
        /// Message header field table
        /// </summary>
        public IDictionary<string, object>? Headers { get; set; }
    }
}
