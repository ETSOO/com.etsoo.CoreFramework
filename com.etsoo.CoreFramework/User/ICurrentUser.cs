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
        /// Unique connection id
        /// 唯一连接编号
        /// </summary>
        string? ConnectionId { get; }

        /// <summary>
        /// Name
        /// 姓名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Avatar
        /// 头像
        /// </summary>
        string? Avatar { get; }

        /// <summary>
        /// Organization id, support switch
        /// 机构编号，可切换
        /// </summary>
        string? Organization { get; }

        /// <summary>
        /// Role value
        /// 角色值
        /// </summary>
        short RoleValue { get; }

        /// <summary>
        /// Universal id
        /// 通用编号
        /// </summary>
        Guid? Uid { get; }

        /// <summary>
        /// Language
        /// 语言
        /// </summary>
        CultureInfo Language { get; }

        /// <summary>
        /// Json data
        /// Json数据
        /// </summary>
        string? JsonData { get; set; }

        /// <summary>
        /// Update
        /// 更新
        /// </summary>
        /// <param name="data">Data collection</param>
        void Update(StringKeyDictionaryObject data);
    }
}