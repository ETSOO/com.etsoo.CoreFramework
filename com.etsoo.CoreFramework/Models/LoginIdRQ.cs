using com.etsoo.WebUtils.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Login id request data
    /// 登录编号请求数据
    /// </summary>
    [JsonDerivedType(typeof(LoginRQ))]
    public record LoginIdRQ
    {
        /// <summary>
        /// Device id
        /// 设备编号
        /// </summary>
        [StringLength(512, MinimumLength = 32)]
        public required string DeviceId { get; init; }

        /// <summary>
        /// Encrypted user's email or mobile
        /// 加密的用户邮箱或者手机号码
        /// </summary>
        [StringLength(256, MinimumLength = 32)]
        public required string Id { get; init; }

        /// <summary>
        /// Country code, like CN = China
        /// 国家编号，如 CN = 中国
        /// </summary>
        [RegionId]
        public required string Region { get; init; }
    }
}
