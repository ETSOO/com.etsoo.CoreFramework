namespace com.etsoo.MessageQueue
{
    /// <summary>
    /// Message properties
    /// 消息属性
    /// </summary>
    public record MessageProperties
    {
        /// <summary>
        /// Application Id
        /// 程序编号
        /// </summary>
        public string? AppId { get; set; }

        /// <summary>
        /// Correlation id
        /// 相关编号
        /// </summary>
        public string? CorrelationId { get; set; }

        /// <summary>
        /// MIME content encoding
        /// MIME 内容编码
        /// </summary>
        public string? ContentEncoding { get; set; }

        /// <summary>
        /// Content type descriptor
        /// 内容类型描述符
        /// </summary>
        public string? ContentType { get; set; }

        /// <summary>
        /// Message priority, 0 to 9
        /// 消息优先级
        /// </summary>
        public byte? Priority { get; set; }

        /// <summary>
        /// Destination to reply to
        /// 回复目标
        /// </summary>
        public string? ReplyTo { get; set; }

        /// <summary>
        /// Message type name
        /// 消息类型
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// User Id
        /// 用户编号
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// The message’s "time to live (TTL)" value
        /// 消息的“生存时间 (TTL)”值
        /// </summary>
        public TimeSpan? TimeToLive { get; set; }

        /// <summary>
        /// Message header field table
        /// 消息头字段表
        /// </summary>
        public IDictionary<string, object>? Headers { get; set; }
    }
}
