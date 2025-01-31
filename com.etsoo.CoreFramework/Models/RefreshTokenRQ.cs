using com.etsoo.CoreFramework.Application;
using com.etsoo.Database.Converters;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Refresh token request data
    /// 刷新令牌请求数据
    /// </summary>
    public record RefreshTokenRQ : IModelValidator
    {
        /// <summary>
        /// Device id
        /// 设备编号
        /// </summary>
        public required string DeviceId { get; init; }

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
            if (DeviceId.Length is not (>= 32 and <= 512))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(DeviceId));
            }

            if (!TimeZoneUtils.IsTimeZone(TimeZone))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(TimeZone));
            }

            return null;
        }
    }
}