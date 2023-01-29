using Microsoft.AspNetCore.Http;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Service user accessor
    /// 服务用户访问器
    /// </summary>
    public class ServiceUserAccessor : UserAccessor<IServiceUser>, IServiceUserAccessor
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="httpContextAccessor">Http context accessor</param>
        public ServiceUserAccessor(IHttpContextAccessor httpContextAccessor)
            : base(ServiceUser.Create(httpContextAccessor.HttpContext))
        {
        }
    }
}
