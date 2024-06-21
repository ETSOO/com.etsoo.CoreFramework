using com.etsoo.WebUtils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace com.etsoo.CoreFramework.User
{
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
    }
}