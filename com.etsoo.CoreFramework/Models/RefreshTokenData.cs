namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Refresh token data
    /// 刷新令牌数据
    /// </summary>
    public record RefreshTokenData
    {
        /// <summary>
        /// User agent
        /// 用户代理
        /// </summary>
        public string? UserAgent { get; init; }

        /// <summary>
        /// Device id
        /// 设备编号
        /// </summary>
        public required string DeviceId { get; init; }

        /// <summary>
        /// Refresh token
        /// 刷新令牌
        /// </summary>
        public required string Token { get; init; }

        /// <summary>
        /// Time zone
        /// 时区
        /// </summary>
        public required string TimeZone { get; init; }
    }
}
