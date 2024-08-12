using com.etsoo.CoreFramework.Authentication;
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
        string? Uid { get; }

        /// <summary>
        /// Service
        /// 服务
        /// </summary>
        string? Service { get; set; }
    }
}