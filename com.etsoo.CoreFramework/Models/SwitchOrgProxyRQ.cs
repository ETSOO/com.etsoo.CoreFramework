using com.etsoo.CoreFramework.Application;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Crypto;
using com.etsoo.Utils.String;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Switch organization proxy request
    /// 切换机构代理请求
    /// </summary>
    public record SwitchOrgProxyRQ
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
        /// Target organization id
        /// 目标机构编号
        /// </summary>
        public required int OrganizationId { get; init; }

        /// <summary>
        /// From organization id
        /// 来源机构编号
        /// </summary>
        public int? FromOrganizationId { get; init; }

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
                [nameof(OrganizationId)] = OrganizationId.ToString(),
                [nameof(FromOrganizationId)] = FromOrganizationId?.ToString() ?? string.Empty
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

            if (Sign.Length is not (>= 32 and <= 512))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Sign));
            }

            return null;
        }
    }
}
