using com.etsoo.CoreFramework.Authentication;
using com.etsoo.WebUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// User accessor extentions
    /// 用户访问器扩展
    /// </summary>
    public static class UserAccessorExtentions
    {
        /// <summary>
        /// Create user from HttpContext
        /// 从HttpContext创建用户
        /// </summary>
        /// <typeparam name="U">Generic user type</typeparam>
        /// <param name="context">HTTP context</param>
        /// <param name="connectionId">Connection id</param>
        /// <returns>Result</returns>
        public static U? CreateUser<U>(this HttpContext context, string? connectionId = null) where U : MinUserToken, IMinUserCreator<U>
        {
            connectionId ??= context.Connection.Id;
            return U.Create(context.User, connectionId);
        }

        /// <summary>
        /// Create user from authorization header
        /// 从授权头创建用户
        /// </summary>
        /// <typeparam name="U">Generic user type</typeparam>
        /// <param name="context">HTTP context</param>
        /// <param name="authService">Authorization service</param>
        /// <param name="audience">Token audience</param>
        /// <param name="schema">Authorization schema</param>
        /// <param name="connectionId">Connection id</param>
        /// <returns>Result</returns>
        public static U? CreateUserFromAuthorization<U>(this HttpContext context, IAuthService authService, string? audience = null, string schema = "Bearer", string? connectionId = null) where U : MinUserToken, IMinUserCreator<U>
        {
            // Authorization header
            var authorization = context.Request.Headers.Authorization;
            if (AuthenticationHeaderValue.TryParse(authorization, out var header) && !string.IsNullOrEmpty(header.Parameter) && header.Scheme.Equals(schema))
            {
                connectionId ??= context.Connection.Id;
                return CreateUserFromToken<U>(authService, header.Parameter, audience, connectionId);
            }

            return default;
        }

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
        public static U? CreateUserFromToken<U>(this IAuthService authService, string token, string? audience = null, string? connectionId = null) where U : MinUserToken, IMinUserCreator<U>
        {
            try
            {
                var (claims, _) = authService.ValidateToken(token, audience);
                if (claims != null)
                {
                    return U.Create(claims, connectionId);
                }
            }
            catch
            {
            }

            return default;
        }
    }

    /// <summary>
    /// User accessor abstract class
    /// 用户访问器抽象类
    /// </summary>
    /// <remarks>
    /// Constructor
    /// 构造函数
    /// </remarks>
    /// <param name="ip">Current IP</param>
    /// <param name="user">User</param>
    public abstract class UserAccessorAbstract<T>(IPAddress ip, T? user) : IUserAccessor<T> where T : IUserCreator<T>
    {
        /// <summary>
        /// Get IP
        /// 获取IP
        /// </summary>
        public IPAddress Ip { get; } = ip;

        /// <summary>
        /// Get user
        /// 获取用户
        /// </summary>
        public T? User { get; } = user;

        /// <summary>
        /// Get non-null user
        /// 获取非空用户
        /// </summary>
        public T UserSafe
        {
            get
            {
                if (User == null)
                {
                    throw new UnauthorizedAccessException();
                }
                return User;
            }
        }

        /// <summary>
        /// Create user from authorization header
        /// 从授权头创建用户
        /// </summary>
        /// <typeparam name="U">Generic user type</typeparam>
        /// <param name="authService">Authorization service</param>
        /// <param name="audience">Token audience</param>
        /// <param name="schema">Authorization schema</param>
        /// <param name="connectionId">Connection id</param>
        public abstract U? CreateUserFromAuthorization<U>(IAuthService authService, string? audience = null, string schema = "Bearer", string? connectionId = null) where U : MinUserToken, IMinUserCreator<U>;

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
        public U? CreateUserFromToken<U>(IAuthService authService, string token, string? audience = null, string? connectionId = null) where U : MinUserToken, IMinUserCreator<U>
        {
            return authService.CreateUserFromToken<U>(token, audience, connectionId);
        }
    }

    /// <summary>
    /// User accessor
    /// 用户访问器
    /// </summary>
    /// <remarks>
    /// Constructor
    /// 构造函数
    /// </remarks>
    /// <param name="httpContextAccessor">Http context accessor</param>
    [method: ActivatorUtilitiesConstructor]
    /// <summary>
    /// User accessor
    /// 用户访问器
    /// </summary>
    public class UserAccessor<T>(IHttpContextAccessor httpContextAccessor)
        : UserAccessorAbstract<T>(
            httpContextAccessor.HttpContext?.RemoteIpAddress() ?? throw new Exception("No IP for user accessor"),
            T.Create(httpContextAccessor.HttpContext?.User, httpContextAccessor.HttpContext?.Connection.Id)
        ) where T : IUserCreator<T>
    {
        public override U? CreateUserFromAuthorization<U>(IAuthService authService, string? audience = null, string schema = "Bearer", string? connectionId = null) where U : class
        {
            return httpContextAccessor.HttpContext?.CreateUserFromAuthorization<U>(authService, audience, schema, connectionId);
        }
    }

    /// <summary>
    /// User accessor for Minimal Api
    /// 最小接口用户访问器
    /// </summary>
    /// <remarks>
    /// Constructor
    /// 构造函数
    /// </remarks>
    /// <param name="context">HttpContext</param>
    [method: ActivatorUtilitiesConstructor]
    /// <summary>
    /// User accessor
    /// 用户访问器
    /// </summary>
    public class UserAccessorMinimal<T>(HttpContext context)
        : UserAccessorAbstract<T>(
            context.RemoteIpAddress() ?? throw new Exception("No IP for user accessor"),
            T.Create(context.User, context.Connection.Id)
        ) where T : IUserCreator<T>
    {
        public override U? CreateUserFromAuthorization<U>(IAuthService authService, string? audience = null, string schema = "Bearer", string? connectionId = null) where U : class
        {
            return context.CreateUserFromAuthorization<U>(authService, audience, schema, connectionId);
        }
    }
}