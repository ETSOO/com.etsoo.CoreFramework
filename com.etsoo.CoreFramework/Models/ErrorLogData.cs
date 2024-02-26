namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Front-end error logging data
    /// 前端错误日志数据
    /// </summary>
    public record ErrorLogData
    {
        /// <summary>
        /// Error type
        /// 错误类型
        /// </summary>
        public required string Type { get; init; }

        /// <summary>
        /// Event type
        /// 事件类型
        /// </summary>
        public string? EventType { get; init; }

        /// <summary>
        /// Message
        /// 消息
        /// </summary>
        public required string Message { get; init; }

        /// <summary>
        /// Source
        /// 源
        /// </summary>
        public string? Source { get; init; }

        /// <summary>
        /// Line number
        /// 行号
        /// </summary>
        public int? LineNo { get; init; }

        /// <summary>
        /// Column number
        /// 列号
        /// </summary>
        public int? ColNo { get; init; }

        /// <summary>
        /// Stack
        /// 栈
        /// </summary>
        public string? Stack { get; init; }
    }
}
