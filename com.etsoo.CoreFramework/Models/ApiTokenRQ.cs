using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// API token request data
    /// 接口令牌请求数据
    /// </summary>
    [JsonDerivedType(typeof(ApiRefreshTokenRQ))]
    public record ApiTokenRQ
    {
        /// <summary>
        /// Refresh token
        /// 刷新令牌
        /// </summary>
        [StringLength(512, MinimumLength = 32)]
        public required string Token { get; init; }
    }

    /// <summary>
    /// API Refresh token request data
    /// 接口刷新令牌请求数据
    /// </summary>
    public record ApiRefreshTokenRQ : ApiTokenRQ
    {
        /// <summary>
        /// Application ID
        /// 程序编号
        /// </summary>
        public required int AppId { get; init; }
    }
}