using com.etsoo.Utils.String;
using System.Globalization;
using System.Net;
using System.Security.Claims;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Current user interface
    /// 当前用户接口
    /// </summary>
    public interface ICurrentUser
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
        /// Id, struct only, string id should be replaced by GUID to avoid sensitive data leak
        /// 编号，结构类型，字符串类型的编号，应该替换为GUID，避免敏感信息泄露
        /// </summary>
        string Id { get; }

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
        /// Client IP address
        /// 客户端IP地址
        /// </summary>
        IPAddress ClientIp { get; }

        /// <summary>
        /// Language
        /// 语言
        /// </summary>
        CultureInfo Language { get; }

        /// <summary>
        /// Country or region
        /// 国家或地区
        /// </summary>
        string Region { get; }

        /// <summary>
        /// Create claims
        /// 创建声明
        /// </summary>
        /// <returns>Claims</returns>
        IEnumerable<Claim> CreateClaims();

        /// <summary>
        /// Create identity
        /// 创建身份
        /// </summary>
        /// <returns>Identity</returns>
        ClaimsIdentity CreateIdentity();

        /// <summary>
        /// Update
        /// 更新
        /// </summary>
        /// <param name="data">Data collection</param>
        void Update(StringKeyDictionaryObject data);
    }
}