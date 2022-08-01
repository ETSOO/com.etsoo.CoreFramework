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
        [Required]
        [StringLength(512, MinimumLength = 32)]
        public string DeviceId { get; init; } = null!;

        /// <summary>
        /// Current password
        /// 当前密码
        /// </summary>
        [StringLength(512, MinimumLength = 64)]
        public string OldPassword { get; init; } = null!;

        /// <summary>
        /// New password
        /// 新密码
        /// </summary>
        [StringLength(512, MinimumLength = 64)]
        public string Password { get; init; } = null!;
    }
}
