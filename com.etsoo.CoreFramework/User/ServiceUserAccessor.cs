using Microsoft.AspNetCore.Http;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Service user accessor
    /// 服务用户访问器
    /// </summary>
    public class ServiceUserAccessor<T> : IServiceUserAccessor<T> where T : class, IServiceUser
    {
        /// <summary>
        /// Http Context
        /// HTTP上下文
        /// </summary>
        protected readonly HttpContext? context;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="context">Http context</param>
        public ServiceUserAccessor(HttpContext? context)
        {
            this.context = context;
        }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="httpContextAccessor">Http context accessor</param>
        public ServiceUserAccessor(IHttpContextAccessor httpContextAccessor) : this(httpContextAccessor.HttpContext)
        {
        }

        /// <summary>
        /// Get user
        /// 获取用户
        /// </summary>
        public T? User => T.Create(context) as T;

        /// <summary>
        /// Get non-null user
        /// 获取非空用户
        /// </summary>
        public T UserSafe
        {
            get
            {
                var user = User;
                if (user == null)
                {
                    throw new UnauthorizedAccessException();
                }
                return user;
            }
        }
    }
}
