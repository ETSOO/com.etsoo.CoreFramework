namespace com.etsoo.CoreFramework.Authentication
{
    /// <summary>
    /// Role extension
    /// 角色扩展
    /// </summary>
    public static class RoleExtensions
    {
        /// <summary>
        /// Get all roles
        /// 获取所有角色
        /// </summary>
        /// <param name="role">Role</param>
        /// <returns>String roles</returns>
        public static IEnumerable<string> GetRoles<E>(this E role) where E : struct, Enum
        {
            return Enum.GetValues<UserRole>().Where(v => role.HasFlag(v)).Select(r => r.ToString());
        }
    }
}
