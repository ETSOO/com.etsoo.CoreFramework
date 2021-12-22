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
        /// Serverside identifier, database device id encrypted
        /// 服务器端识别码，数据库端加密的设备编号
        /// </summary>
        public string? Identifier { get; init; }

        /// <summary>
        /// Timestamp, JavaScript miliseconds
        /// 时间戳，JavaScript毫秒数
        /// </summary>
        public long Timestamp { get; init; }
    }
}
