using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// User accessor
    /// 用户访问器
    /// </summary>
    /// <remarks>
    /// Constructor
    /// 构造函数
    /// </remarks>
    /// <param name="ip">Current IP</param>
    /// <param name="user">User</param>
    public class UserAccessor<T>(IPAddress ip, T? user) : IUserAccessor<T> where T : IUserCreator<T>
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
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="httpContextAccessor">Http context accessor</param>
        [ActivatorUtilitiesConstructor]
        public UserAccessor(IHttpContextAccessor httpContextAccessor)
            : this(httpContextAccessor.HttpContext?.Connection.RemoteIpAddress ?? throw new ArgumentNullException("No IP for user accessor"),
                  T.Create(httpContextAccessor.HttpContext?.User, httpContextAccessor.HttpContext?.Connection.Id))
        {
        }
    }
}
