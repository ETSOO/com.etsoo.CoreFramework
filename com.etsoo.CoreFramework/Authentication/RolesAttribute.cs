using com.etsoo.WebUtils.Attributes;
using System.Runtime.Versioning;

namespace com.etsoo.CoreFramework.Authentication
{
    /// <summary>
    /// User roles attribute
    /// 用户角色属性
    /// </summary>
    [RequiresPreviewFeatures]
    public class RolesAttribute : BaseRolesAttribute<UserRole>
    {
        public RolesAttribute(UserRole role) : base(role)
        {
        }
    }
}
