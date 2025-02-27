using com.etsoo.CoreFramework.Application;
using com.etsoo.Utils.Actions;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Switch organization proxy request
    /// 切换机构代理请求
    /// </summary>
    public record SwitchOrgProxyRQ : SignModel
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

            return null;
        }
    }
}
