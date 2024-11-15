namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Antiforgery token
    /// 反伪造令牌
    /// </summary>
    public record AntiforgeryToken
    {
        /// <summary>
        /// Token hold field, header, or cookie name
        /// 令牌保存字段，头部或者Cookie名称
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Token value
        /// 令牌值
        /// </summary>
        public required string Value { get; init; }
    }
}
