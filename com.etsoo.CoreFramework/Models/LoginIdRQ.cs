using com.etsoo.CoreFramework.Application;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;
using System.Text.Json.Serialization;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Login id request data
    /// 登录编号请求数据
    /// </summary>
    [JsonDerivedType(typeof(LoginRQ))]
    public record LoginIdRQ : IModelValidator
    {
        /// <summary>
        /// Device id
        /// 设备编号
        /// </summary>
        public required string DeviceId { get; init; }

        /// <summary>
        /// Encrypted user's email or mobile
        /// 加密的用户邮箱或者手机号码
        /// </summary>
        public required string Id { get; init; }

        /// <summary>
        /// Country code, like CN = China
        /// 国家编号，如 CN = 中国
        /// </summary>
        public required string Region { get; init; }

        /// <summary>
        /// Validate the model
        /// 验证模块
        /// </summary>
        /// <returns>Result</returns>
        public virtual IActionResult? Validate()
        {
            if (DeviceId.Length is not (>= 32 and <= 512))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(DeviceId));
            }

            if (Id.IndexOf('@') > 0)
            {
                if (Id.Length is not (>= 6 and <= 512))
                {
                    return ApplicationErrors.NoValidData.AsResult(nameof(Id));
                }
            }
            else
            {
                if (Id.Length is not (>= 6 and <= 20))
                {
                    return ApplicationErrors.NoValidData.AsResult(nameof(Id));
                }
            }

            if (Region.Length is not 2)
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Region));
            }

            return null;
        }
    }
}
