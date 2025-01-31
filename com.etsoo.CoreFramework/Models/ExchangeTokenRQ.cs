using com.etsoo.CoreFramework.Application;
using com.etsoo.Database.Converters;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Exchange token request data
    /// 令牌交换请求数据
    /// </summary>
    public record ExchangeTokenRQ : IModelValidator
    {
        /// <summary>
        /// Token from core system
        /// 核心系统的令牌
        /// </summary>
        public required string Token { get; init; }

        /// <summary>
        /// Time zone
        /// 时区
        /// </summary>
        public required string TimeZone { get; init; }

        /// <summary>
        /// Validate the model
        /// 验证模块
        /// </summary>
        /// <returns>Result</returns>
        public IActionResult? Validate()
        {
            if (Token.Length is not (>= 32 and <= 512))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Token));
            }

            if (!TimeZoneUtils.IsTimeZone(TimeZone))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(TimeZone));
            }

            return null;
        }
    }
}
