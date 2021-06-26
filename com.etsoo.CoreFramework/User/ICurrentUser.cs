using System;
using System.Collections.Generic;
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
        string Name { get; set; }

        /// <summary>
        /// Avatar
        /// 头像
        /// </summary>
        string? Avatar { get; set; }

        /// <summary>
        /// Id
        /// 编号
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Int type id
        /// 整形编号
        /// </summary>
        int IntId { get; }

        /// <summary>
        /// Guid type id
        /// Guid类型编号
        /// </summary>
        Guid GuidId { get; }

        /// <summary>
        /// Role
        /// 角色
        /// </summary>
        IEnumerable<string> Roles { get; }

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
    }
}
