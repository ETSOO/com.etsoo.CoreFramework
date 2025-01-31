using com.etsoo.CoreFramework.Application;
using com.etsoo.Database.Converters;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Crypto;
using com.etsoo.Utils.Models;
using com.etsoo.Utils.String;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Authentication refresh token request
    /// 认证刷新令牌请求
    /// </summary>
    public record AuthRefreshTokenRQ : IModelValidator
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
        /// Refresh token
        /// 刷新令牌
        /// </summary>
        public required string RefreshToken { get; init; }

        /// <summary>
        /// Time zone
        /// 时区
        /// </summary>
        public required string TimeZone { get; init; }

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
                [nameof(RefreshToken)] = RefreshToken,
                [nameof(TimeZone)] = TimeZone
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
            if (AppKey.Length > 0 && AppKey.Length is not (>= 32 and <= 128))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(AppKey));
            }

            if (RefreshToken.Length is not (>= 64 and <= 512))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(RefreshToken));
            }

            if (Sign.Length is not (>= 64 and <= 1024))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Sign));
            }

            if (!TimeZoneUtils.IsTimeZone(TimeZone))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(TimeZone));
            }

            return null;
        }
    }
}
