using com.etsoo.CoreFramework.Application;
using com.etsoo.Utils;
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
        /// Timestamp, long big number may cause JSON serialization issue
        /// 时间戳，长整数可能导致JSON序列化问题
        /// </summary>
        public string Timestamp { get; set; } = string.Empty;

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
            if (string.IsNullOrEmpty(Timestamp))
            {
                Timestamp = SharedUtils.UTCToUnixSeconds().ToString();
            }
            rq[nameof(Timestamp)] = Timestamp;

            // With an extra '&' at the end
            var query = rq.JoinAsString().TrimEnd('&');

            return Convert.ToHexString(CryptographyUtils.HMACSHA256(query, appSecret));
        }

        /// <summary>
        /// Total minutes from the timestamp
        /// 从时间戳获取总分钟数
        /// </summary>
        /// <returns>Total minutes</returns>
        public double TotalMinutes() => DateTime.UtcNow.Subtract(SharedUtils.UnixSecondsToUTC(long.Parse(Timestamp))).TotalMinutes;

        /// <summary>
        /// Validate the model
        /// 验证模块
        /// </summary>
        /// <returns>Result</returns>
        public virtual IActionResult? Validate()
        {
            if (!long.TryParse(Timestamp, out _))
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
