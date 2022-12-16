using Microsoft.AspNetCore.Http;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Service user accessor
    /// 服务用户访问器
    /// </summary>
    public class ServiceUserAccessor : IServiceUserAccessor
    {
        private readonly HttpContext? context;

        public ServiceUserAccessor(HttpContext? context)
        {
            this.context = context;
        }

        public ServiceUserAccessor(IHttpContextAccessor httpContextAccessor) : this(httpContextAccessor.HttpContext)
        {
        }

        /// <summary>
        /// Get user
        /// 获取用户
        /// </summary>
        public IServiceUser? User => ServiceUser.Create(context);

        /// <summary>
        /// Get non-null user
        /// 获取非空用户
        /// </summary>
        public IServiceUser UserSafe => ServiceUser.CreateSafe(context);
    }
}
