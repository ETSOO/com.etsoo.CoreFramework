using com.etsoo.CoreFramework.Authentication;
using System.Net;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// User accessor interface
    /// 用户访问器接口
    /// </summary>
    public interface IUserAccessor<T> where T : IUserCreator<T>
    {
        /// <summary>
        /// Get IP
        /// 获取IP
        /// </summary>
        IPAddress Ip { get; }

        /// <summary>
        /// Get user
        /// 获取用户
        /// </summary>
        T? User { get; }

        /// <summary>
        /// Get non-null user
        /// 获取非空用户
        /// </summary>
        T UserSafe { get; }

        /// <summary>
        /// Create user from authorization header
        /// 从授权头创建用户
        /// </summary>
        /// <typeparam name="U">Generic user type</typeparam>
        /// <param name="authService">Authorization service</param>
        /// <param name="audience">Token audience</param>
        /// <param name="schema">Authorization schema</param>
        /// <param name="connectionId">Connection id</param>
        U? CreateUserFromAuthorization<U>(IAuthService authService, string? audience = null, string schema = "Bearer", string? connectionId = null) where U : MinUserToken, IMinUserCreator<U>;

        /// <summary>
        /// Create user from authorization token
        /// 从授权令牌创建用户
        /// </summary>
        /// <typeparam name="U">Generic user type</typeparam>
        /// <param name="authService">Authorization service</param>
        /// <param name="token">Token</param>
        /// <param name="audience">Audience</param>
        /// <param name="connectionId">Connection id</param>
        /// <returns>Result</returns>
        U? CreateUserFromToken<U>(IAuthService authService, string token, string? audience = null, string? connectionId = null) where U : MinUserToken, IMinUserCreator<U>;
    }
}