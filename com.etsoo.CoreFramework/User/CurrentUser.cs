using com.etsoo.CoreFramework.Authentication;
using com.etsoo.Utils.String;
using System.Globalization;
using System.Net;
using System.Security.Claims;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Current user data, more information has to be done with additional API call
    /// 当前用户数据，更多信息需要额外的API调用
    /// </summary>
    public record CurrentUser : UserToken, ICurrentUser, IUserCreator<CurrentUser>
    {
        /// <summary>
        /// Avatar claim type
        /// 头像声明类型
        /// </summary>
        public const string AvatarClaim = "avatar";

        /// <summary>
        /// Organization name claim type
        /// 机构名称申明类型
        /// </summary>
        public const string OrganizationNameClaim = "OrgName";

        /// <summary>
        /// Create user
        /// 创建用户
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <param name="connectionId">Connection id</param>
        /// <returns>User</returns>
        public new static CurrentUser? Create(ClaimsPrincipal? claims, string? connectionId = null)
        {
            if (claims == null) return null;

            var user = UserToken.Create(claims);
            if (user == null) return null;

            // Claims
            var name = claims.FindFirstValue(ClaimTypes.Name);
            var orgName = claims.FindFirstValue(OrganizationNameClaim);
            var avatar = claims.FindFirstValue(AvatarClaim);
            var language = claims.FindFirstValue(ClaimTypes.Locality);
            var roleValue = StringUtils.TryParse<short>(claims.FindFirstValue(ServiceUser.RoleValueClaim)).GetValueOrDefault();

            // Validate
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(language))
                return null;

            // New user
            return new CurrentUser(
                user.Id,
                user.Scopes,
                user.Organization,
                name, roleValue,
                user.ClientIp,
                user.DeviceId,
                new CultureInfo(language),
                user.Region,
                connectionId)
            {
                Avatar = avatar,
                OrganizationName = orgName
            };
        }

        private static (string? id, IEnumerable<string>? scopes, string? organization, short? Role, string? deviceId, string? Name, string? OrgName, string? Avatar) GetData(StringKeyDictionaryObject data)
        {
            return (
                data.Get("Id"),
                data.GetArray("Scopes"),
                data.Get("Organization"),
                data.Get<short>("Role"),
                data.Get("DeviceId"),
                data.Get("Name"),
                data.Get("OrgName"),
                data.Get("Avatar")
            );
        }

        /// <summary>
        /// Create user from result data
        /// 从操作结果数据创建用户
        /// </summary>
        /// <param name="data">Result data</param>
        /// <param name="ip">Ip address</param>
        /// <param name="language">Language</param>
        /// <param name="region">Country or region</param>
        /// <param name="connectionId">Connection id</param>
        /// <returns>User</returns>
        public static CurrentUser? Create(StringKeyDictionaryObject data, IPAddress ip, CultureInfo language, string region, string? connectionId = null)
        {
            // Get data
            var (id, scopes, organization, role, deviceId, name, orgName, avatar) = GetData(data);

            // Validation
            if (id == null || role == null || string.IsNullOrEmpty(deviceId) || string.IsNullOrEmpty(name))
                return null;

            // New user
            return new CurrentUser(
                id,
                scopes,
                organization,
                name,
                role.Value,
                ip,
                deviceId,
                language,
                region,
                connectionId)
            {
                Avatar = avatar,
                OrganizationName = orgName
            };
        }

        /// <summary>
        /// Name
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Organization name
        /// 机构名称
        /// </summary>
        public string? OrganizationName { get; set; }

        /// <summary>
        /// Avatar
        /// 头像
        /// </summary>
        public string? Avatar { get; private set; }

        /// <summary>
        /// Role value
        /// 角色值
        /// </summary>
        public virtual short RoleValue { get; private set; }

        /// <summary>
        /// Role
        /// 角色
        /// </summary>
        public UserRole? Role { get; private set; }

        /// <summary>
        /// Language
        /// 语言
        /// </summary>
        public CultureInfo Language { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="scopes">Scopes</param>
        /// <param name="organization">Organization</param>
        /// <param name="name">Name</param>
        /// <param name="roleValue">Role value</param>
        /// <param name="clientIp">Client IP</param>
        /// <param name="deviceId">Device id</param>
        /// <param name="language">Language</param>
        /// <param name="region">Country or region</param>
        /// <param name="connectionId">Connection id</param>
        public CurrentUser(string id, IEnumerable<string>? scopes, string? organization, string name, short roleValue, IPAddress clientIp, string deviceId, CultureInfo language, string region, string? connectionId = null)
            : base(id, scopes, clientIp, region, deviceId, organization, connectionId)
        {
            Name = name;
            RoleValue = roleValue;
            Role = ServiceUser.GetRole(roleValue);
            Language = language;
        }

        /// <summary>
        /// Create claims
        /// 创建声明
        /// </summary>
        /// <returns>Claims</returns>
        protected override List<Claim> CreateClaims()
        {
            var claims = base.CreateClaims();

            claims.AddRange([
                new(ClaimTypes.Name, Name),
                new(ClaimTypes.Locality, Language.Name),
                new(ServiceUser.RoleValueClaim, RoleValue.ToString())
            ]);

            if (!string.IsNullOrEmpty(OrganizationName))
                claims.Add(new(OrganizationNameClaim, OrganizationName));

            if (Avatar != null)
                claims.Add(new(AvatarClaim, Avatar));

            return claims;
        }

        /// <summary>
        /// Update
        /// 更新
        /// </summary>
        /// <param name="data">Data collection</param>
        public virtual void Update(StringKeyDictionaryObject data)
        {
            // Editable fields
            var (_, scopes, _, role, _, name, orgName, avatar) = GetData(data);

            // Scopes
            Scopes = scopes;

            // Role
            if (role != null && RoleValue != role)
                RoleValue = role.Value;

            // Name
            if (name != null)
                Name = name;

            OrganizationName = orgName;

            // Avatar
            Avatar = avatar;
        }
    }
}
