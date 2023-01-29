﻿namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// User accessor
    /// 用户访问器
    /// </summary>
    public class UserAccessor<T> : IUserAccessor<T> where T : IServiceUser
    {
        /// <summary>
        /// Get user
        /// 获取用户
        /// </summary>
        public T? User { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="user">User</param>
        public UserAccessor(T? user)
        {
            User = user;
        }

        /// <summary>
        /// Get non-null user
        /// 获取非空用户
        /// </summary>
        public T UserSafe
        {
            get
            {
                if (User == null)
                {
                    throw new UnauthorizedAccessException();
                }
                return User;
            }
        }
    }
}
