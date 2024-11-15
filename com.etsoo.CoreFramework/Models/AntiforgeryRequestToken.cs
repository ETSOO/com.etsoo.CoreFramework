namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Antiforgery token
    /// 反伪造令牌
    /// </summary>
    public record AntiforgeryRequestToken
    {
        /// <summary>
        /// Name of the form field used for the request token
        /// 用于请求令牌的表单字段名称
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Name of the header used for the request token
        /// 用于请求令牌的标头名称
        /// </summary>
        public required string? HeaderName { get; init; }

        /// <summary>
        /// Request token value
        /// 请求令牌值
        /// </summary>
        public required string? Value { get; init; }
    }
}
