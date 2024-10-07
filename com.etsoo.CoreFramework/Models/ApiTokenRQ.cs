using System.ComponentModel.DataAnnotations;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// API Refresh token request data
    /// 接口刷新令牌请求数据
    /// </summary>
    public record ApiTokenRQ
    {
        /// <summary>
        /// Refresh token
        /// 刷新令牌
        /// </summary>
        [StringLength(512, MinimumLength = 32)]
        public required string Token { get; init; }
    }
}