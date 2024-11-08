using com.etsoo.CoreFramework.Application;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Models;
using System.Text.Json.Serialization;

namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// API token request data
    /// 接口令牌请求数据
    /// </summary>
    [JsonDerivedType(typeof(ApiRefreshTokenRQ))]
    public record ApiTokenRQ : IModelValidator
    {
        /// <summary>
        /// Refresh token
        /// 刷新令牌
        /// </summary>
        public required string Token { get; init; }

        /// <summary>
        /// Validate the model
        /// 验证模块
        /// </summary>
        /// <returns>Result</returns>
        public IActionResult? Validate()
        {
            if (Token.Length is not (>= 32 and <= 512))
            {
                return ApplicationErrors.NoValidData.AsResult(nameof(Token));
            }

            return null;
        }
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