using com.etsoo.CoreFramework.User;
using System.Security.Claims;

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
        /// Refresh token expiration days
        /// 刷新令牌到期时间（天）
        /// </summary>
        int RefreshTokenDays { get; }

        /// <summary>
        /// Create token
        /// 创建令牌
        /// </summary>
        /// <param name="action">Action</param>
        /// <returns>Token</returns>
        public string CreateToken(AuthAction action);

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
        /// <param name="user">User</param>
        /// <returns>Token</returns>
        string CreateRefreshToken(ICurrentUser user);

        /// <summary>
        /// Validate refresh token
        /// 验证刷新令牌
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>Claims</returns>
        ClaimsPrincipal? ValidateRefreshToken(string token);
    }
}
