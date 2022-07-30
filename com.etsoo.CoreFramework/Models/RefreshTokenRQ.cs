using System.ComponentModel.DataAnnotations;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Refresh token request data
    /// 刷新令牌请求数据
    /// </summary>
    public record RefreshTokenRQ
    {
        /// <summary>
        /// Device id
        /// 设备编号
        /// </summary>
        [Required]
        [StringLength(512, MinimumLength = 32)]
        public string DeviceId { get; init; } = null!;

        /// <summary>
        /// Login password
        /// 登录密码
        /// </summary>
        [Required]
        [StringLength(512, MinimumLength = 64)]
        public string? Pwd;

        /// <summary>
        /// Service id
        /// 服务编号
        /// </summary>
        public int? ServiceId { get; init; }

        /// <summary>
        /// Service Uid
        /// 服务全局编号
        /// </summary>
        public Guid? ServiceUid { get; init; }

        /// <summary>
        /// Timezone name
        /// 时区名称
        /// </summary>
        public string? Timezone { get; init; }
    }
}
