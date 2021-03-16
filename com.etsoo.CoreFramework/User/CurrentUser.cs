using com.etsoo.Utils.String;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security.Claims;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Current user data
    /// 当前用户数据
    /// </summary>
    public record CurrentUser(string Id, string Name, IEnumerable<string> Roles, IPAddress ClientIp, CultureInfo Language, string? ConnectionId) : ICurrentUser
    {
        /// <summary>
        /// Create user
        /// 创建用户
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="connectionId">Connection id</param>
        /// <returns>User</returns>
        public static CurrentUser? Create(ClaimsPrincipal? user, string? connectionId = null)
        {
            // Basic check
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
                return null;

            // Claims
            var name = user.FindFirstValue(ClaimTypes.Name);
            var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var language = user.FindFirstValue(ClaimTypes.Locality);
            var role = user.FindFirstValue(ClaimTypes.Role);
            var ip = user.FindFirstValue(IPAddressClaim);

            // Validate
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(id) || language == null || ip == null || !IPAddress.TryParse(ip, out var ipAddress))
                return null;

            // Roles
            var roles = string.IsNullOrEmpty(role) ? Array.Empty<string>() : role.Split(',');

            // New user
            return new CurrentUser(id, name, roles, ipAddress, new CultureInfo(language), connectionId);
        }

        /// <summary>
        /// Create user from result data
        /// 从操作结果数据创建用户
        /// </summary>
        /// <param name="data">Result data</param>
        /// <param name="ip">Ip address</param>
        /// <param name="language">Language</param>
        /// <returns>User</returns>
        public static CurrentUser? Create(StringKeyDictionaryObject data, IPAddress ip, CultureInfo language)
        {
            // Get data
            var id = data.Get("Id");
            var name = data.Get("Name");
            var role = data.Get("Role");

            // Validation
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(id))
                return null;

            // Roles
            var roles = string.IsNullOrEmpty(role) ? Array.Empty<string>() : role.Split(',');

            // New user
            return new CurrentUser(id, name, roles, ip, language, null);
        }

        /// <summary>
        /// IP Address claim type
        /// IP地址声明类型
        /// </summary>
        public const string IPAddressClaim = "ipaddress";

        /// <summary>
        /// Create claims
        /// 创建声明
        /// </summary>
        /// <returns>Claims</returns>
        public virtual IEnumerable<Claim> CreateClaims()
        {
            return new Claim[] {
                new (ClaimTypes.Name, Name),
                new (ClaimTypes.NameIdentifier, Id),
                new (ClaimTypes.Locality, Language.Name),
                new (ClaimTypes.Role, string.Join(',', Roles)),
                new (IPAddressClaim, ClientIp.ToString())
            };
        }

        /// <summary>
        /// Create identity
        /// 创建身份
        /// </summary>
        /// <returns>Identity</returns>
        public virtual ClaimsIdentity CreateIdentity()
        {
            return new ClaimsIdentity(CreateClaims());
        }
    }
}
