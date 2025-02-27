using com.etsoo.CoreFramework.Application;
using com.etsoo.Utils.Actions;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Authentication request
    /// 认证请求
    /// </summary>
    public record AuthRequest : SignModel
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
        // [AllowedValues(CodeResponseType, TokenResponseType)]
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
        /// Sign the request with the app secret
        /// 对请求使用应用密钥签名
        /// </summary>
        /// <param name="appSecret">Application secret</param>
        /// <returns>Result</returns>
        public string SignWith(string appSecret)
        {
            var rq = new SortedDictionary<string, string>()
            {
                { nameof(AppId), AppId.ToString() },
                { nameof(AppKey), AppKey },
                { nameof(RedirectUri), RedirectUri.ToString() },
                { nameof(ResponseType), ResponseType },
                { nameof(Scope), Scope },
                { nameof(State), State }
            };

            if (!string.IsNullOrEmpty(AccessType))
            {
                rq.Add(nameof(AccessType), AccessType);
            }

            if (!string.IsNullOrEmpty(LoginHint))
            {
                rq.Add(nameof(LoginHint), LoginHint);
            }

            return SignWith(rq, appSecret);
        }

        /// <summary>
        /// Validate the model
        /// 验证模块
        /// </summary>
        /// <returns>Result</returns>
        public override IActionResult? Validate()
        {
            var result = base.Validate();
            if (result != null)
            {
                return result;
            }

            if (AppKey.Length > 0 && AppKey.Length is not (>= 32 and <= 128))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(AppKey));
            }

            if (LoginHint != null && LoginHint.Length is not (>= 1 and <= 256))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(LoginHint));
            }

            if (AccessType != null && AccessType.Length is not (>= 1 and <= 32))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(AccessType));
            }

            if (ResponseType != CodeResponseType && ResponseType != TokenResponseType)
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(ResponseType));
            }

            if (State.Length is not (>= 1 and <= 512))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(State));
            }

            return null;
        }
    }
}
