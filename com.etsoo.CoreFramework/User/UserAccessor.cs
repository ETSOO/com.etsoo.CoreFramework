using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

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
    /// <param name="user">User</param>
    public class UserAccessor<T>(T? user) : IUserAccessor<T> where T : IUserCreator<T>
    {
        /// <summary>
        /// Get user
        /// 获取用户
        /// </summary>
        public T? User { get; } = user;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="httpContextAccessor">Http context accessor</param>
        [ActivatorUtilitiesConstructor]
        public UserAccessor(IHttpContextAccessor httpContextAccessor)
            : this(T.Create(httpContextAccessor.HttpContext?.User, httpContextAccessor.HttpContext?.Connection.Id))
        {
        }

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
    /// Service user accessor
    /// 服务用户访问器
    /// </summary>
    /// <param name="user">Service user</param>
    public class UserAccessor(ServiceUser? user) : UserAccessor<ServiceUser>(user), IUserAccessor
    {
    }

    /// <summary>
    /// Current user accessor
    /// 当前用户访问器
    /// </summary>
    /// <param name="user">Current user</param>
    public class CurrentUserAccessor(CurrentUser? user) : UserAccessor<CurrentUser>(user), ICurrentUserAccessor
    {
    }
}
