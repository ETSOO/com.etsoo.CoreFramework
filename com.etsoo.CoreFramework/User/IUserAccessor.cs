namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// User accessor interface
    /// 用户访问器接口
    /// </summary>
    public interface IUserAccessor<T> where T : IUserCreator<T>
    {
        /// <summary>
        /// Get user
        /// 获取用户
        /// </summary>
        T? User { get; }

        /// <summary>
        /// Get non-null user
        /// 获取非空用户
        /// </summary>
        T UserSafe { get; }
    }

    /// <summary>
    /// Service user accessor interface
    /// 服务用户访问器接口
    /// </summary>
    public interface IUserAccessor : IUserAccessor<ServiceUser>
    {
    }

    /// <summary>
    /// Current user accessor interface
    /// 当前用户访问器接口
    /// </summary>
    public interface ICurrentUserAccessor : IUserAccessor<CurrentUser>
    {
    }
}
