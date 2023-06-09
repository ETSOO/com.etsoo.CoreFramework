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
        /// </summary>
        public string MessageId { get; set; } = string.Empty;

        /// <summary>
        /// Correlation id
        /// </summary>
        public string? CorrelationId { get; set; }

        /// <summary>
        /// Application id.
        /// </summary>
        public string? AppId { get; set; }

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
        /// User id
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Timestamp
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Message header field table
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
