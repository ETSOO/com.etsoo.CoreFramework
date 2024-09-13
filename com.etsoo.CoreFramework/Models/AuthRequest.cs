using com.etsoo.Utils.Crypto;
using com.etsoo.Utils.String;
using System.ComponentModel.DataAnnotations;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Authentication request
    /// 认证请求
    /// </summary>
    public record AuthRequest
    {
        /// <summary>
        /// Code response type
        /// 代码响应类型
        /// </summary>
        public const string CodeResponseType = "code";

        /// <summary>
        /// Token response type
        /// 令牌响应类型
        /// </summary>
        public const string TokenResponseType = "token";

        /// <summary>
        /// Offline access type
        /// 离线访问类型
        /// </summary>
        public const string OfflineAccessType = "offline";

        /// <summary>
        /// Application ID
        /// 应用编号
        /// </summary>
        public required int AppId { get; init; }

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
        /// Access type
        /// 访问类型
        /// </summary>
        public string? AccessType { get; init; }

        /// <summary>
        /// Redirect URI
        /// 重定向URI
        /// </summary>
        public required Uri RedirectUri { get; init; }

        /// <summary>
        /// Response type, code or token
        /// 响应类型，代码或令牌
        /// </summary>
        [AllowedValues(CodeResponseType, TokenResponseType)]
        public required string ResponseType { get; init; }

        /// <summary>
        /// Space-delimited permission scope(s)
        /// 空格分隔的权限范围
        /// </summary>
        public required string Scope { get; set; }

        /// <summary>
        /// Scopes
        /// 范围
        /// </summary>
        public IEnumerable<string> Scopes
        {
            get
            {
                return Scope.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            }
            set
            {
                Scope = string.Join(' ', value);
            }
        }

        /// <summary>
        /// State value
        /// 状态值
        /// </summary>
        public required string State { get; init; }

        /// <summary>
        /// Signature
        /// 签名
        /// </summary>
        public string Sign { get; set; } = string.Empty;

        /// <summary>
        /// Sign the request with the app secret
        /// 对请求使用应用密钥签名
        /// </summary>
        /// <param name="appSecret">Application secret</param>
        /// <returns>Result</returns>
        public string SignWith(string appSecret)
        {
            var rq = new SortedDictionary<string, string>()
            {
                { "scope", Scope },
                { "responseType", ResponseType },
                { "state", State },
                { "redirectUri", RedirectUri.ToString() },
                { "appId", AppId.ToString() },
                { "appKey", AppKey }
            };

            if (!string.IsNullOrEmpty(AccessType))
            {
                rq.Add("accessType", AccessType);
            }

            if (!string.IsNullOrEmpty(LoginHint))
            {
                rq.Add("loginHint", LoginHint);
            }

            // With an extra '&' at the end
            var query = rq.JoinAsString().TrimEnd('&');

            return Convert.ToHexString(CryptographyUtils.HMACSHA256(query, appSecret));
        }
    }
}
