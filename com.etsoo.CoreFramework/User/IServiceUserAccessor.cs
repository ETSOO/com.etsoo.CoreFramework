namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Service user accessor interface
    /// 服务用户访问器接口
    /// </summary>
    public interface IServiceUserAccessor
    {
        /// <summary>
        /// Get user
        /// 获取用户
        /// </summary>
        IServiceUser? User { get; }

        /// <summary>
        /// Get non-null user
        /// 获取非空用户
        /// </summary>
        IServiceUser UserSafe { get; }
    }
}
