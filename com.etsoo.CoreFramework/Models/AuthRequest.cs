namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Authentication request
    /// 认证请求
    /// </summary>
    public record AuthRequest
    {
        /// <summary>
        /// Application ID
        /// 应用编号
        /// </summary>
        public required string AppId { get; init; }

        /// <summary>
        /// Application key
        /// 应用键值
        /// </summary>
        public required string AppKey { get; init; }

        /// <summary>
        /// Login hint (user login name)
        /// 登录提示（个人登录名）
        /// </summary>
        public string? LoginHint { get; init; }

        /// <summary>
        /// Redirect URI
        /// 重定向URI
        /// </summary>
        public required string RedirectUri { get; init; }

        /// <summary>
        /// Response type, code or token
        /// 响应类型，代码或令牌
        /// </summary>
        public required string ResponseType { get; init; }

        /// <summary>
        /// Scope
        /// 作用域
        /// </summary>
        public required string Scope { get; init; }

        /// <summary>
        /// State value
        /// 状态值
        /// </summary>
        public required string State { get; init; }

        /// <summary>
        /// Signature
        /// 签名
        /// </summary>
        public required string Sign { get; init; }
    }
}
