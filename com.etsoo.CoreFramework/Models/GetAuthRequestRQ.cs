using com.etsoo.CoreFramework.Application;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Get authentication request data
    /// 获取认证请求数据
    /// </summary>
    public record GetAuthRequestRQ : IModelValidator
    {
        /// <summary>
        /// Region
        /// 地区
        /// </summary>
        public required string Region { get; init; }

        /// <summary>
        /// Device
        /// 设备
        /// </summary>
        public required string Device { get; init; }

        /// <summary>
        /// Validate the model
        /// 验证模块
        /// </summary>
        /// <returns>Result</returns>
        public virtual IActionResult? Validate()
        {
            if (Region.Length is not 2)
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Region));
            }

            if (Device.Length is not (>= 1 and <= 512))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Device));
            }

            return null;
        }
    }
}
