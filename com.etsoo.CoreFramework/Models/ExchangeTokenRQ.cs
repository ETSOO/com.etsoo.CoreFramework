using System.ComponentModel.DataAnnotations;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Exchange token request data
    /// 令牌交换请求数据
    /// </summary>
    public record ExchangeTokenRQ
    {
        /// <summary>
        /// Token from core system
        /// 核心系统的令牌
        /// </summary>
        [Required]
        public string Token { get; init; } = null!;
    }
}
