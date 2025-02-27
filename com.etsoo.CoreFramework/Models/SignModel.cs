using com.etsoo.CoreFramework.Application;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Crypto;
using com.etsoo.Utils.Models;
using com.etsoo.Utils.String;
using System.Text.Json.Serialization;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Sign model
    /// 签名模型
    /// </summary>
    [JsonDerivedType(typeof(AuthCreateTokenRQ))]
    [JsonDerivedType(typeof(AuthRefreshTokenRQ))]
    [JsonDerivedType(typeof(AuthRequest))]
    [JsonDerivedType(typeof(LoginStateRQ))]
    [JsonDerivedType(typeof(SwitchOrgProxyRQ))]
    public abstract record SignModel : IModelValidator
    {
        /// <summary>
        /// Timestamp
        /// 时间戳
        /// </summary>
        public long Timestamp { get; set; }

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
        public string SignWith(SortedDictionary<string, string> rq, string appSecret)
        {
            if (Timestamp == 0)
            {
                Timestamp = DateTime.UtcNow.ToBinary();
            }
            rq[nameof(Timestamp)] = Timestamp.ToString();

            // With an extra '&' at the end
            var query = rq.JoinAsString().TrimEnd('&');

            return Convert.ToHexString(CryptographyUtils.HMACSHA256(query, appSecret));
        }

        /// <summary>
        /// Total minutes from the timestamp
        /// 从时间戳获取总分钟数
        /// </summary>
        public double TotalMinutes => DateTime.UtcNow.Subtract(DateTime.FromBinary(Timestamp)).TotalMinutes;

        /// <summary>
        /// Validate the model
        /// 验证模块
        /// </summary>
        /// <returns>Result</returns>
        public virtual IActionResult? Validate()
        {
            if (Timestamp < 1)
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Timestamp));
            }

            if (Sign.Length is not (>= 32 and <= 512))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Sign));
            }

            return null;
        }
    }
}
