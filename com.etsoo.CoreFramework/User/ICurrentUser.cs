using com.etsoo.CoreFramework.Authentication;
using com.etsoo.Utils.String;
using System.Globalization;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Current user interface
    /// 当前用户接口
    /// </summary>
    public interface ICurrentUser : IUserToken
    {
        /// <summary>
        /// Name
        /// 姓名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Organization name
        /// 机构名称
        /// </summary>
        string? OrganizationName { get; }

        /// <summary>
        /// Avatar
        /// 头像
        /// </summary>
        string? Avatar { get; }

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
        /// Update
        /// 更新
        /// </summary>
        /// <param name="data">Data collection</param>
        void Update(StringKeyDictionaryObject data);
    }
}