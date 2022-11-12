using System.ComponentModel.DataAnnotations;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Change password request data
    /// 修改密码请求数据
    /// </summary>
    public record ChangePasswordRQ
    {
        /// <summary>
        /// Device id
        /// 设备编号
        /// </summary>
        [StringLength(512, MinimumLength = 32)]
        public required string DeviceId { get; init; }

        /// <summary>
        /// Current password
        /// 当前密码
        /// </summary>
        [StringLength(512, MinimumLength = 64)]
        public required string OldPassword { get; init; }

        /// <summary>
        /// New password
        /// 新密码
        /// </summary>
        [StringLength(512, MinimumLength = 64)]
        public required string Password { get; init; }
    }
}
