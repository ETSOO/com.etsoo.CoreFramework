using com.etsoo.WebUtils.Attributes;

namespace com.etsoo.CoreFramework.Authentication
{
    /// <summary>
    /// User roles attribute
    /// 用户角色属性
    /// </summary>
    public class RolesAttribute : BaseRolesAttribute<UserRole>
    {
        public RolesAttribute(UserRole role) : base(role)
        {
        }
    }
}
