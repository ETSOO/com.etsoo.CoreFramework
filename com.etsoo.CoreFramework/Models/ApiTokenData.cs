namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// API token data
    /// 接口令牌数据
    /// </summary>
    public record ApiTokenData
    {
        /// <summary>
        /// A token that can be sent to API for access
        /// 可以发送到 API 以获取访问权限的令牌
        /// </summary>
        public required string AccessToken { get; init; }

        /// <summary>
        /// The token type. Always set to Bearer
        /// 令牌类型。始终设置为 Bearer
        /// </summary>
        public string TokenType { get; init; } = "Bearer";

        /// <summary>
        /// The remaining lifetime of the access token in seconds
        /// 访问令牌的剩余生存时间（以秒为单位）
        /// </summary>
        public required int ExpiresIn { get; init; }

        /// <summary>
        /// Refresh token
        /// 刷新令牌
        /// </summary>
        public virtual required string RefreshToken { get; init; }
    }
}
