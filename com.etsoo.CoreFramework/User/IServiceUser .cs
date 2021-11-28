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
        /// Organization id, support switch
        /// 机构编号，可切换
        /// </summary>
        string? Organization { get; }

        /// <summary>
        /// Int id
        /// 整数编号
        /// </summary>
        int IdInt { get; }

        /// <summary>
        /// Int organization id
        /// 整数机构编号
        /// </summary>
        int? OrganizationInt { get; }

        /// <summary>
        /// Role value
        /// 角色值
        /// </summary>
        short RoleValue { get; }

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