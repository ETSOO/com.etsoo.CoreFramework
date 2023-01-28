﻿using com.etsoo.CoreFramework.Authentication;
using com.etsoo.Utils.String;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Net;

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
        static virtual IServiceUser? Create(HttpContext? context) => null;

        /// <summary>
        /// Create non nullable user
        /// 创建非空用户
        /// </summary>
        /// <param name="context">Http context</param>
        /// <returns>Service user</returns>
        static virtual IServiceUser CreateSafe(HttpContext? context) => throw new UnauthorizedAccessException();

        /// <summary>
        /// Create user from result data
        /// 从操作结果数据创建用户
        /// </summary>
        /// <param name="data">Result data</param>
        /// <param name="ip">Ip address</param>
        /// <param name="language">Language</param>
        /// <param name="region">Country or region</param>
        /// <returns>User</returns>
        static virtual IServiceUser? Create(StringKeyDictionaryObject data, IPAddress ip, CultureInfo language, string region) => null;

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