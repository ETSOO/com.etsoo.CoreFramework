﻿namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Application token data
    /// 程序令牌数据
    /// </summary>
    public record AppTokenData
    {
        /// <summary>
        /// A token that can be sent to API for access
        /// 可以发送到 API 以获取访问权限的令牌
        /// </summary>
        public required string AccessToken { get; init; }

        /// <summary>
        /// The token type. Current always set to Bearer
        /// 令牌类型。当前始终设置为 Bearer
        /// </summary>
        public string TokenType { get; init; } = "Bearer";

        /// <summary>
        /// The remaining lifetime of the access token in seconds
        /// 访问令牌的剩余生存时间（以秒为单位）
        /// </summary>
        public required int ExpiresIn { get; init; }

        /// <summary>
        /// This field is only present if the access_type parameter was set to offline in the authentication request
        /// 该字段仅在身份验证请求中的 access_type 参数设置为 offline 时存在
        /// </summary>
        public string? RefreshToken { get; init; }

        /// <summary>
        /// The scopes of access granted by the access_token expressed as a list of space-delimited, case-sensitive strings
        /// 用于访问 access_token 授予的访问权限的范围，表示为一组以空格分隔的区分大小写的字符串
        /// </summary>
        public required string Scope { get; init; }

        /// <summary>
        /// A JWT that contains identity information about the user that is digitally signed by the system
        /// scope with 'openid' will contain an ID token
        /// JWT 包含由系统数字签名的有关用户的身份信息
        /// </summary>
        public string? IdToken { get; init; }
    }
}
