using Microsoft.AspNetCore.Http;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Service user accessor
    /// 服务用户访问器
    /// </summary>
    public class ServiceUserAccessor : UserAccessor<ServiceUser>, IServiceUserAccessor
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="context">Http context</param>
        public ServiceUserAccessor(HttpContext? context) : base(context)
        {
        }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="httpContextAccessor">Http context accessor</param>
        public ServiceUserAccessor(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }
    }
}
