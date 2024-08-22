using com.etsoo.CoreFramework.Authentication;
using com.etsoo.Utils;
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
        /// Get role from value
        /// 从值获取角色
        /// </summary>
        /// <param name="roleValue">Role value</param>
        /// <returns>User role</returns>
        public static UserRole? GetRole(short roleValue)
        {
            var userRole = (UserRole)roleValue;
            return SharedUtils.EnumIsDefined(userRole) ? userRole : null;
        }

        /// <summary>
        /// Avatar claim type
        /// 头像声明类型
        /// </summary>
        public const string AvatarClaim = "avatar";

        /// <summary>
        /// Name claim type
        /// 姓名声明类型
        /// </summary>
        public const string NameClaim = "name";

        /// <summary>
        /// Locality claim type
        /// 本地化声明类型
        /// </summary>
        public const string LocalityClaim = "locality";

        /// <summary>
        /// Organization name claim type
        /// 机构名称申明类型
        /// </summary>
        public const string OrganizationNameClaim = "orgname";

        /// <summary>
        /// Role value claim type
        /// 角色值声明类型
        /// </summary>
        public const string RoleValueClaim = "role";

        /// <summary>
        /// Organization user id claim type
        /// 机构用户编号申明类型
        /// </summary>
        public const string OidClaim = "oid";

        /// <summary>
        /// Channel organization claim type
        /// 渠道机构声明类型
        /// </summary>
        public const string ChannelOrganizationClaim = "channelorganization";

        /// <summary>
        /// Parent organization claim type
        /// 父机构声明类型
        /// </summary>
        public const string ParentOrganizationClaim = "parentorganization";

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
            var name = claims.FindFirstValue(NameClaim);
            var orgName = claims.FindFirstValue(OrganizationNameClaim);
            var oid = claims.FindFirstValue(OidClaim);
            var avatar = claims.FindFirstValue(AvatarClaim);
            var language = claims.FindFirstValue(LocalityClaim);
            var roleValue = StringUtils.TryParse<short>(claims.FindFirstValue(RoleValueClaim)).GetValueOrDefault();
            var channelOrganization = claims.FindFirstValue(ChannelOrganizationClaim);
            var parentOrganization = claims.FindFirstValue(ParentOrganizationClaim);

            // Validate
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(oid) || string.IsNullOrEmpty(language))
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
                oid,
                avatar,
                orgName,
                channelOrganization,
                parentOrganization,
                connectionId);
        }

        private static (string? id, IEnumerable<string>? scopes, string? organization, short? Role, string? deviceId, string? name, string? orgName, string? oid, string? avatar, string? channelOrganization, string? parentOrganization) GetData(StringKeyDictionaryObject data)
        {
            return (
                data.Get("Id"),
                data.GetArray("Scopes"),
                data.Get("Organization"),
                data.Get<short>("Role"),
                data.Get("DeviceId"),
                data.Get("Name"),
                data.Get("OrgName"),
                data.Get("Oid"),
                data.Get("Avatar"),
                data.Get("ChannelOrganization"),
                data.Get("ParentOrganization")
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
            var (id, scopes, organization, role, deviceId, name, orgName, oid, avatar, channelOrganization, parentOrganization) = GetData(data);

            // Validation
            if (id == null || role == null || string.IsNullOrEmpty(organization) || string.IsNullOrEmpty(oid) || string.IsNullOrEmpty(deviceId) || string.IsNullOrEmpty(name))
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
                oid,
                avatar,
                orgName,
                channelOrganization,
                parentOrganization,
                connectionId);
        }

        /// <summary>
        /// Name
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Organization user Id
        /// 机构用户编号
        /// </summary>
        public string Oid { get; }

        /// <summary>
        /// Int organization user id
        /// 整数机构用户编号
        /// </summary>
        public int OidInt { get; }

        /// <summary>
        /// Organization name
        /// 机构名称
        /// </summary>
        public string? OrganizationName { get; set; }

        /// <summary>
        /// Channel organization id
        /// 渠道机构编号
        /// </summary>
        public string? ChannelOrganization { get; }

        /// <summary>
        /// Int channel organization id
        /// 整数渠道机构编号
        /// </summary>
        public int? ChannelOrganizationInt { get; }

        /// <summary>
        /// Parent organization id
        /// 父机构编号
        /// </summary>
        public string? ParentOrganization { get; }

        /// <summary>
        /// Int parent organization id
        /// 整数父机构编号
        /// </summary>
        public int? ParentOrganizationInt { get; }

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
        /// <param name="oid">Organization user id</param>
        /// <param name="avatar">Avatar</param>
        /// <param name="orgName">Organization name</param>
        /// <param name="channelOrganization">Channel organization</param>
        /// <param name="parentOrganization">Parent organization id</param>
        /// <param name="connectionId">Connection id</param>
        public CurrentUser(string id, IEnumerable<string>? scopes, string organization, string name, short roleValue,
            IPAddress clientIp, string deviceId, CultureInfo language, string region, string oid,
            string? avatar, string? orgName, string? channelOrganization, string? parentOrganization, string? connectionId = null)
            : base(id, scopes, clientIp, region, deviceId, organization, connectionId)
        {
            Name = name;
            RoleValue = roleValue;
            Role = GetRole(roleValue);
            Language = language;
            Oid = oid;
            Avatar = avatar;
            OrganizationName = orgName;
            ChannelOrganization = channelOrganization;
            ParentOrganization = parentOrganization;

            if (int.TryParse(oid, out var oidValue))
            {
                OidInt = oidValue;
            }

            if (int.TryParse(channelOrganization, out var channelOrganizationValue))
            {
                ChannelOrganizationInt = channelOrganizationValue;
            }

            if (int.TryParse(parentOrganization, out var parentOrganizationValue))
            {
                ParentOrganizationInt = parentOrganizationValue;
            }
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
                new(NameClaim, Name),
                new(LocalityClaim, Language.Name),
                new(RoleValueClaim, RoleValue.ToString()),
                new(OidClaim, Oid)
            ]);

            if (!string.IsNullOrEmpty(OrganizationName))
                claims.Add(new(OrganizationNameClaim, OrganizationName));

            if (!string.IsNullOrEmpty(ChannelOrganization))
                claims.Add(new(ChannelOrganizationClaim, ChannelOrganization));

            if (!string.IsNullOrEmpty(ParentOrganization))
                claims.Add(new(ParentOrganizationClaim, ParentOrganization));

            if (Avatar != null)
                claims.Add(new(AvatarClaim, Avatar));

            return claims;
        }
    }
}
