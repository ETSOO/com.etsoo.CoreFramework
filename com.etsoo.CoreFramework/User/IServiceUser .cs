using com.etsoo.CoreFramework.Authentication;
using com.etsoo.Utils.String;
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
        Guid? Uid { get; }

        /// <summary>
        /// Service
        /// 服务
        /// </summary>
        string? Service { get; set; }

        /// <summary>
        /// Update
        /// 更新
        /// </summary>
        /// <param name="data">Data collection</param>
        void Update(StringKeyDictionaryObject data);
    }
}