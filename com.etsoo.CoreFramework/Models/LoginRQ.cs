using System.ComponentModel.DataAnnotations;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Login request data
    /// 登录请求数据
    /// </summary>
    public record LoginRQ : LoginIdRQ
    {
        /// <summary>
        /// Login password
        /// 登录密码
        /// </summary>
        [StringLength(512, MinimumLength = 64)]
        public required string Pwd { get; init; }

        /// <summary>
        /// Specific servce id, or uid includes organization data
        /// 具体服务编号，或包括机构信息的全局编号
        /// </summary>
        public string? ServiceId { get; init; }

        /// <summary>
        /// Timezone name
        /// 时区名称
        /// </summary>
        public string? Timezone { get; init; }
    }
}
