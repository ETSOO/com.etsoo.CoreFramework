using com.etsoo.CoreFramework.Application;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Crypto;
using com.etsoo.Utils.Models;
using com.etsoo.Utils.String;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Authentication token creation request
    /// 认证令牌创建请求
    /// </summary>
    public record AuthCreateTokenRQ : IModelValidator
    {
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
        /// Authorization code
        /// 授权代码
        /// </summary>
        public required string Code { get; init; }

        /// <summary>
        /// Redirect URI
        /// 重定向URI
        /// </summary>
        public required Uri RedirectUri { get; init; }

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
            var rq = new SortedDictionary<string, string>
            {
                [nameof(AppId)] = AppId.ToString(),
                [nameof(AppKey)] = AppKey,
                [nameof(Code)] = Code,
                [nameof(RedirectUri)] = RedirectUri.ToString()
            };

            // With an extra '&' at the end
            var query = rq.JoinAsString().TrimEnd('&');

            return Convert.ToHexString(CryptographyUtils.HMACSHA256(query, appSecret));
        }

        /// <summary>
        /// Validate the model
        /// 验证模块
        /// </summary>
        /// <returns>Result</returns>
        public IActionResult? Validate()
        {
            if (AppKey.Length is not (>= 32 and <= 128))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(AppKey));
            }

            if (Code.Length is not (>= 32 and <= 128))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Code));
            }

            if (Sign.Length is not (>= 64 and <= 512))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Sign));
            }

            return null;
        }
    }
}
