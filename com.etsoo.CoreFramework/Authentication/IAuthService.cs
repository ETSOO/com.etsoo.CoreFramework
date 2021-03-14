using com.etsoo.CoreFramework.User;
using System;
using System.Net;

namespace com.etsoo.CoreFramework.Authentication
{
    /// <summary>
    /// Authentification service
    /// 验证服务
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Access token expiration minutes
        /// 访问令牌到期时间（分钟)
        /// </summary>
        int AccessTokenMinutes { get; }

        /// <summary>
        /// Refresh token expiration hours
        /// 刷新令牌到期时间（小时）
        /// </summary>
        int RefreshTokenHours { get; }

        /// <summary>
        /// Create token
        /// 创建令牌
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="liveSpan">Live time span</param>
        /// <returns>Token</returns>
        string CreateToken(ICurrentUser user, TimeSpan liveSpan);

        /// <summary>
        /// Create access token
        /// 创建访问令牌
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Token</returns>
        string CreateAccessToken(ICurrentUser user);

        /// <summary>
        /// Create refresh token
        /// 创建刷新令牌
        /// </summary>
        /// <returns>Token</returns>
        string CreateRefreshToken();
    }
}
