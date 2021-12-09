namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Init call request data
    /// 初始化调用请求数据
    /// </summary>
    public record InitCallRQ
    {
        /// <summary>
        /// Device id
        /// 设备编号
        /// </summary>
        public string? DeviceId { get; init; }

        /// <summary>
        /// Timestamp, JavaScript miliseconds
        /// 时间戳，JavaScript毫秒数
        /// </summary>
        public long Timestamp { get; init; }
    }
}
