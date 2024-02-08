using com.etsoo.Utils.String;

namespace com.etsoo.MessageQueue
{
    /// <summary>
    /// Message received properties
    /// 接收到的消息属性
    /// </summary>
    public record MessageReceivedProperties
    {
        /// <summary>
        /// Message id
        /// 消息编号
        /// </summary>
        public string MessageId { get; set; } = string.Empty;

        /// <summary>
        /// Correlation id
        /// 相关编号
        /// </summary>
        public string? CorrelationId { get; set; }

        /// <summary>
        /// Application id
        /// 程序编号
        /// </summary>
        public string? AppId { get; set; }

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
        /// User id
        /// 用户编号
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Timestamp
        /// 时间戳
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Cache data
        /// 缓存数据
        /// </summary>
        public object? CacheData { get; set; }

        /// <summary>
        /// Message header field table
        /// 消息头字段表
        /// </summary>
        public IDictionary<string, object>? Headers { get; set; }

        /// <summary>
        /// Get header item value
        /// 获取表头项值
        /// </summary>
        /// <param name="key">Item key</param>
        /// <returns>Result</returns>
        public string? GetHeader(string key)
        {
            if (Headers?.TryGetValue(key, out var value) is true)
            {
                return Convert.ToString(value);
            }

            return default;
        }

        /// <summary>
        /// Get header item value
        /// 获取表头项值
        /// </summary>
        /// <typeparam name="T">Generic value type</typeparam>
        /// <param name="key">Item key</param>
        /// <returns>Result</returns>
        public T? GetHeader<T>(string key) where T : struct
        {
            if (Headers?.TryGetValue(key, out var value) is true)
            {
                return StringUtils.TryParseObject<T>(value);
            }

            return default;
        }
    }
}
