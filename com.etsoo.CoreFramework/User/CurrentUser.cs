using com.etsoo.Utils.String;
using System.Globalization;
using System.Net;
using System.Security.Claims;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Current user data
    /// 当前用户数据
    /// </summary>
    public record CurrentUser : ICurrentUser
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
        /// Organization claim type
        /// 机构声明类型
        /// </summary>
        public const string OrganizationClaim = "Organization";

        /// <summary>
        /// Role value claim type
        /// 角色值类型
        /// </summary>
        public const string RoleValueClaim = "RoleValue";

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
            var avatar = user.FindFirstValue(AvatarClaim);
            var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
            var organization = user.FindFirstValue(OrganizationClaim);
            var language = user.FindFirstValue(ClaimTypes.Locality);
            var country = user.FindFirstValue(ClaimTypes.Country);
            var roleValue = StringUtils.TryParse<short>(user.FindFirstValue(RoleValueClaim)).GetValueOrDefault();
            var ip = user.FindFirstValue(IPAddressClaim);

            // Validate
            if (string.IsNullOrEmpty(name) || id == null || language == null || country == null || ip == null || !IPAddress.TryParse(ip, out var ipAddress))
                return null;

            // New user
            return new CurrentUser(id, organization, name, roleValue, ipAddress, new CultureInfo(language), country, connectionId)
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
        /// <param name="country">Country or region</param>
        /// <returns>User</returns>
        public static CurrentUser? Create(StringKeyDictionaryObject data, IPAddress ip, CultureInfo language, string country)
        {
            // Get data
            var id = data.Get("Id");
            var organization = data.Get("Organization");
            var name = data.Get("Name");
            var roleValue = data.Get<short>("Role").GetValueOrDefault();

            // Validation
            if (id == null || string.IsNullOrEmpty(name))
                return null;

            // New user
            return new CurrentUser(id, organization, name, roleValue, ip, language, country, null)
            {
                Avatar = data.Get("Avatar")
            };
        }

        /// <summary>
        /// Raw id
        /// 原始编号
        /// </summary>
        public string RawId { get; }

        /// <summary>
        /// Id, struct only, string id should be replaced by GUID to avoid sensitive data leak
        /// 编号，结构类型，字符串类型的编号，应该替换为GUID，避免敏感信息泄露
        /// </summary>
        public virtual object Id => RawId;

        /// <summary>
        /// Raw organization id
        /// 原始机构编号
        /// </summary>
        public string? RawOrganization { get; set; }

        /// <summary>
        /// Organization id, support switch
        /// 机构编号，可切换
        /// </summary>
        public virtual object? Organization => RawOrganization;

        /// <summary>
        /// Name
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Role value
        /// 角色值
        /// </summary>
        public virtual short RoleValue { get; set; }

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
        /// Country or region, like CN means China
        /// 国家和地区，比如 CN = 中国
        /// </summary>
        public string Country { get; }

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
        /// <param name="organization">Organization</param>
        /// <param name="name">Name</param>
        /// <param name="roleValue">Role value</param>
        /// <param name="clientIp">Client IP</param>
        /// <param name="language">Language</param>
        /// <param name="country">Country or region</param>
        /// <param name="connectionId">Connection id</param>
        public CurrentUser(string id, string? organization, string name, short roleValue, IPAddress clientIp, CultureInfo language, string country, string? connectionId)
        {
            RawId = id;
            RawOrganization = organization;
            Name = name;
            RoleValue = roleValue;
            ClientIp = clientIp;
            Language = language;
            Country = country;
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
            yield return new(ClaimTypes.NameIdentifier, RawId);
            yield return new(ClaimTypes.Locality, Language.Name);
            yield return new(ClaimTypes.Country, Country);
            yield return new(RoleValueClaim, RoleValue.ToString());
            yield return new(IPAddressClaim, ClientIp.ToString());
            if (!string.IsNullOrEmpty(RawOrganization))
                yield return new(OrganizationClaim, RawOrganization);
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
