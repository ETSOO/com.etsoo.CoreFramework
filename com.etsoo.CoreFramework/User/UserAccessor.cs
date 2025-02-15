using com.etsoo.CoreFramework.Authentication;
using com.etsoo.WebUtils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        /// <param name="reason">Failure reason</param>
        /// <returns>Result</returns>
        public static U? CreateUser<U>(this HttpContext context, out string? reason) where U : MinUserToken, IMinUserCreator<U>
        {
            return U.Create(context.User, out reason);
        }

        /// <summary>
        /// Create user from claims
        /// 从声明创建用户
        /// </summary>
        /// <typeparam name="U">Generic user type</typeparam>
        /// <param name="claims">Claims</param>
        /// <param name="logger">Logger</param>
        /// <returns>Result</returns>
        public static U? CreateUser<U>(this HttpContext context, ILogger logger) where U : IUserCreator<U>
        {
            var user = U.Create(context.User, out var reason);

            if (reason != null && context.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() == null)
            {
                // Ignored the meanless reason when no user
                var claims = context.User?.Claims.Select(claim => $"{claim.Type} = {claim.Value}");
                var claimsString = claims == null ? null : string.Join(", ", claims);
                logger.LogWarning("Create user failed: {reason} with {isAuthenticated} claims {claims}", reason, context.User?.Identity?.IsAuthenticated, claimsString);
            }

            return user;
        }

        /// <summary>
        /// Create user from authorization header
        /// 从授权头创建用户
        /// </summary>
        /// <typeparam name="U">Generic user type</typeparam>
        /// <param name="context">HTTP context</param>
        /// <param name="authService">Authorization service</param>
        /// <param name="reason">Failure reason</param>
        /// <param name="audience">Token audience</param>
        /// <param name="schema">Authorization schema</param>
        /// <returns>Result</returns>
        public static U? CreateUserFromAuthorization<U>(this HttpContext context, IAuthService authService, out string? reason, string? audience = null, string schema = "Bearer") where U : MinUserToken, IMinUserCreator<U>
        {
            // Authorization header
            var authorization = context.Request.Headers.Authorization;

            if (AuthenticationHeaderValue.TryParse(authorization, out var header))
            {
                if (header.Scheme.Equals(schema))
                {
                    if (!string.IsNullOrEmpty(header.Parameter))
                    {
                        return CreateUserFromToken<U>(authService, header.Parameter, out reason, audience);
                    }
                    else
                    {
                        reason = "NoAuthorizationToken";
                    }
                }
                else
                {
                    reason = "InvalidAuthorizationSchema:" + header.Scheme;
                }
            }
            else
            {
                // Ignore no authorization header
                reason = null;
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
        /// <param name="reason">Failure reason</param>
        /// <param name="audience">Audience</param>
        /// <returns>Result</returns>
        public static U? CreateUserFromToken<U>(this IAuthService authService, string token, out string? reason, string? audience = null) where U : MinUserToken, IMinUserCreator<U>
        {
            try
            {
                var (claims, _) = authService.ValidateToken(token, audience);
                return U.Create(claims, out reason);
            }
            catch (Exception ex)
            {
                reason = "Exception:" + ex.Message;
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
                    throw new UnauthorizedAccessException("UserAccessor");
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
        /// <param name="reason">Failure reason</param>
        /// <param name="audience">Token audience</param>
        /// <param name="schema">Authorization schema</param>
        public abstract U? CreateUserFromAuthorization<U>(IAuthService authService, out string? reason, string? audience = null, string schema = "Bearer") where U : MinUserToken, IMinUserCreator<U>;

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
        public U? CreateUserFromToken<U>(IAuthService authService, string token, out string? reason, string? audience = null) where U : MinUserToken, IMinUserCreator<U>
        {
            return authService.CreateUserFromToken<U>(token, out reason, audience);
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
    /// <summary>
    /// User accessor
    /// 用户访问器
    /// </summary>
    public class UserAccessor<T>(IHttpContextAccessor httpContextAccessor, ILogger logger)
        : UserAccessorAbstract<T>(
            httpContextAccessor.HttpContext?.RemoteIpAddress() ?? throw new Exception("No IP for user accessor"),
            httpContextAccessor.HttpContext.CreateUser<T>(logger)
        ) where T : IUserCreator<T>
    {
        public override U? CreateUserFromAuthorization<U>(IAuthService authService, out string? reason, string? audience = null, string schema = "Bearer") where U : class
        {
            var context = httpContextAccessor.HttpContext;
            if (context == null)
            {
                reason = null;
                return default;
            }

            return context.CreateUserFromAuthorization<U>(authService, out reason, audience, schema);
        }
    }

    /// <summary>
    /// Current user accessor
    /// 当前用户访问器
    /// </summary>
    public class CurrentUserAccessor : UserAccessor<CurrentUser>
    {
        [ActivatorUtilitiesConstructor]
        public CurrentUserAccessor(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUserAccessor> logger)
            : base(httpContextAccessor, logger)
        {
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
    /// <summary>
    /// User accessor
    /// 用户访问器
    /// </summary>
    public class UserAccessorMinimal<T>(HttpContext context, ILogger logger)
        : UserAccessorAbstract<T>(
            context.RemoteIpAddress() ?? throw new Exception("No IP for user accessor"),
            context.CreateUser<T>(logger)
        ) where T : IUserCreator<T>
    {
        public override U? CreateUserFromAuthorization<U>(IAuthService authService, out string? reason, string? audience = null, string schema = "Bearer") where U : class
        {
            return context.CreateUserFromAuthorization<U>(authService, out reason, audience, schema);
        }
    }
}