using com.etsoo.CoreFramework.Authentication;
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
        /// Organization user Id
        /// 机构用户编号
        /// </summary>
        string Oid { get; }

        /// <summary>
        /// Int organization user id
        /// 整数机构用户编号
        /// </summary>
        int OidInt { get; }

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
        /// Channel organization id
        /// 渠道机构编号
        /// </summary>
        string? ChannelOrganization { get; }

        /// <summary>
        /// Int channel organization id
        /// 整数渠道机构编号
        /// </summary>
        int? ChannelOrganizationInt { get; }

        /// <summary>
        /// Parent organization id
        /// 父机构编号
        /// </summary>
        string? ParentOrganization { get; }

        /// <summary>
        /// Int parent organization id
        /// 整数父机构编号
        /// </summary>
        int? ParentOrganizationInt { get; }

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
    }
}