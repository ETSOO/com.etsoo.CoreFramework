namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Init call request data
    /// 初始化调用请求数据
    /// </summary>
    public record InitCallRQ
    {
        /// <summary>
        /// Timestamp, JavaScript miliseconds
        /// 时间戳，JavaScript毫秒数
        /// </summary>
        public long Timestamp { get; init; }
    }
}
