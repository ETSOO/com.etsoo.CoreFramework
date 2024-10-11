using System.ComponentModel.DataAnnotations;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Signout request data
    /// 退出系统请求数据
    /// </summary>
    public record SignoutRQ
    {
        /// <summary>
        /// Device id
        /// 设备编号
        /// </summary>
        [StringLength(512, MinimumLength = 32)]
        public required string DeviceId { get; init; }

        /// <summary>
        /// Refresh token
        /// 刷新令牌
        /// </summary>
        [StringLength(512, MinimumLength = 32)]
        public required string Token { get; init; }
    }
}
