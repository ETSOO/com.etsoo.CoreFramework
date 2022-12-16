using com.etsoo.CoreFramework.Authentication;
using com.etsoo.Utils.String;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Service user interface
    /// 服务用户接口
    /// </summary>
    public interface IServiceUser : IUserToken
    {
        /// <summary>
        /// Create user
        /// 创建用户
        /// </summary>
        /// <param name="context">Http context</param>
        /// <returns>Service user</returns>
        static abstract IServiceUser? Create(HttpContext? context);

        /// <summary>
        /// Create non nullable user
        /// 创建非空用户
        /// </summary>
        /// <param name="context">Http context</param>
        /// <returns>Service user</returns>
        static abstract IServiceUser CreateSafe(HttpContext? context);

        /// <summary>
        /// Role value
        /// 角色值
        /// </summary>
        short RoleValue { get; }

        /// <summary>
        /// Role
        /// 角色
        /// </summary>
        UserRole? Role { get; }

        /// <summary>
        /// Language
        /// 语言
        /// </summary>
        CultureInfo Language { get; }

        /// <summary>
        /// User Uid
        /// 用户全局编号
        /// </summary>
        Guid? Uid { get; }

        /// <summary>
        /// Update
        /// 更新
        /// </summary>
        /// <param name="data">Data collection</param>
        void Update(StringKeyDictionaryObject data);
    }
}