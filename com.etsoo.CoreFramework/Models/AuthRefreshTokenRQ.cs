using com.etsoo.CoreFramework.Application;
using com.etsoo.Database.Converters;
using com.etsoo.Utils.Actions;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Authentication refresh token request
    /// 认证刷新令牌请求
    /// </summary>
    public record AuthRefreshTokenRQ : SignModel
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

            if (RefreshToken.Length is not (>= 64 and <= 512))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(RefreshToken));
            }

            if (!TimeZoneUtils.IsTimeZone(TimeZone))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(TimeZone));
            }

            return null;
        }
    }
}
