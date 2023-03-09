namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// User accessor interface
    /// 用户访问器接口
    /// </summary>
    public interface IUserAccessor<T> where T : IServiceUser
    {
        /// <summary>
        /// Cancellation token
        /// 取消令牌
        /// </summary>
        CancellationToken CancellationToken { get; }

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
}
