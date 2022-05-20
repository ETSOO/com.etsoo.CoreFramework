using com.etsoo.Utils;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.Versioning;

namespace com.etsoo.WebUtils.Attributes
{
    /// <summary>
    /// Authorization with roles
    /// 角色授权
    /// </summary>
    [RequiresPreviewFeatures]
    public class BaseRolesAttribute<E> : AuthorizeAttribute where E : struct, Enum
    {
        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="role">Role</param>
        public BaseRolesAttribute(E role)
        {
            Roles = string.Join(",", role.GetKeys());
        }
    }
}