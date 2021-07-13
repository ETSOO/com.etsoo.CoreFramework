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
    public record CurrentUser<T> : ICurrentUser<T> where T : struct
    {
        /// <summary>
        /// IP Address claim type
        /// IP地址声明类型
        /// </summary>
        public const string IPAddressClaim = "ipaddress";

        /// <summary>
        /// Avatar claim type
        /// 头像声明类型
        /// </summary>
        public const string AvatarClaim = "avatar";

        /// <summary>
        /// Create user
        /// 创建用户
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="connectionId">Connection id</param>
        /// <returns>User</returns>
        public static CurrentUser<T>? Create(ClaimsPrincipal? user, string? connectionId = null)
        {
            // Basic check
            if (user == null || user.Identity == null || !user.Identity.IsAuthenticated)
                return null;

            // Claims
            var name = user.FindFirstValue(ClaimTypes.Name);
            var avatar = user.FindFirstValue(AvatarClaim);
            var id = StringUtils.TryParse<T>(user.FindFirstValue(ClaimTypes.NameIdentifier));
            var language = user.FindFirstValue(ClaimTypes.Locality);
            var role = user.FindFirstValue(ClaimTypes.Role);
            var ip = user.FindFirstValue(IPAddressClaim);

            // Validate
            if (string.IsNullOrEmpty(name) || id == null || language == null || ip == null || !IPAddress.TryParse(ip, out var ipAddress))
                return null;

            // Roles
            var roles = string.IsNullOrEmpty(role) ? Array.Empty<string>() : role.Split(',');

            // New user
            return new CurrentUser<T>(id.Value, name, roles, ipAddress, new CultureInfo(language), connectionId)
            {
                Avatar = avatar
            };
        }

        /// <summary>
        /// Create user from result data
        /// 从操作结果数据创建用户
        /// </summary>
        /// <param name="data">Result data</param>
        /// <param name="ip">Ip address</param>
        /// <param name="language">Language</param>
        /// <returns>User</returns>
        public static CurrentUser<T>? Create(StringKeyDictionaryObject data, IPAddress ip, CultureInfo language)
        {
            // Get data
            var id = data.Get<T>("Id");
            var name = data.Get("Name");
            var role = data.Get("Role");

            // Validation
            if (id == null || string.IsNullOrEmpty(name))
                return null;

            // Roles
            var roles = string.IsNullOrEmpty(role) ? Array.Empty<string>() : role.Split(',');

            // New user
            return new CurrentUser<T>(id.Value, name, roles, ip, language, null)
            {
                Avatar = data.Get("Avatar")
            };
        }

        /// <summary>
        /// Id, struct only, string id should be replaced by GUID to avoid sensitive data leak
        /// 编号，结构类型，字符串类型的编号，应该替换为GUID，避免敏感信息泄露
        /// </summary>
        public T Id { get; }

        /// <summary>
        /// Name
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Roles
        /// 角色
        /// </summary>
        public IEnumerable<string> Roles { get; }

        /// <summary>
        /// Client IP
        /// 客户端IP地址
        /// </summary>
        public IPAddress ClientIp { get; }

        /// <summary>
        /// Language
        /// 语言
        /// </summary>
        public CultureInfo Language { get; }

        /// <summary>
        /// Connection id
        /// 链接编号
        /// </summary>
        public string? ConnectionId { get; }

        /// <summary>
        /// Avatar
        /// 头像
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="name">Name</param>
        /// <param name="roles">Roles</param>
        /// <param name="clientIp">Client IP</param>
        /// <param name="language">Language</param>
        /// <param name="connectionId">Connection id</param>
        public CurrentUser(T id, string name, IEnumerable<string> roles, IPAddress clientIp, CultureInfo language, string? connectionId)
        {
            Id = id;
            Name = name;
            Roles = roles;
            ClientIp = clientIp;
            Language = language;
            ConnectionId = connectionId;
        }

        /// <summary>
        /// Create claims
        /// 创建声明
        /// </summary>
        /// <returns>Claims</returns>
        public virtual IEnumerable<Claim> CreateClaims()
        {
            yield return new(ClaimTypes.Name, Name);
            yield return new(ClaimTypes.NameIdentifier, Id.ToString()!);
            yield return new(ClaimTypes.Locality, Language.Name);
            yield return new(ClaimTypes.Role, string.Join(',', Roles));
            yield return new(IPAddressClaim, ClientIp.ToString());
            if (Avatar != null)
                yield return new(AvatarClaim, Avatar);
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
