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
        /// User uid claim type
        /// 用户uid声明类型
        /// </summary>
        public const string UidClaim = "uid";

        /// <summary>
        /// App key claim type
        /// 程序键值声明类型
        /// </summary>
        public const string AppKeyClaim = "appkey";

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

            var user = UserToken.Create(claims, connectionId);
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
            var uid = claims.FindFirstValue(UidClaim);
            var appKey = claims.FindFirstValue(AppKeyClaim);

            // Validate
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(oid) || string.IsNullOrEmpty(language))
                return null;

            // New user
            return new CurrentUser
            {
                ClientIp = user.ClientIp,
                DeviceId = user.DeviceId,
                Id = user.Id,
                Scopes = user.Scopes,
                Region = user.Region,
                Organization = user.Organization,

                Name = name,
                RoleValue = roleValue,
                Language = new CultureInfo(language),
                Oid = oid,
                Avatar = avatar,
                OrganizationName = orgName,
                ChannelOrganization = channelOrganization,
                ParentOrganization = parentOrganization,
                Uid = uid,
                AppKey = appKey
            };
        }

        private static (string? id, IEnumerable<string>? scopes, string? organization, short? Role, string? deviceId, string? name, string? orgName, string? oid, string? avatar, string? channelOrganization, string? parentOrganization, string? uid, string? appKey) GetData(StringKeyDictionaryObject data)
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
                data.Get("ParentOrganization"),
                data.Get("Uid"),
                data.Get("AppKey")
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
            var (id, scopes, organization, role, deviceId, name, orgName, oid, avatar, channelOrganization, parentOrganization, uid, appKey) = GetData(data);

            // Validation
            if (id == null || role == null || string.IsNullOrEmpty(organization) || string.IsNullOrEmpty(oid) || string.IsNullOrEmpty(deviceId) || string.IsNullOrEmpty(name))
                return null;

            // New user
            return new CurrentUser
            {
                ClientIp = ip,
                DeviceId = deviceId,
                Id = id,
                ConnectionId = connectionId,
                Scopes = scopes,
                Region = region,
                Organization = organization,
                Name = name,
                RoleValue = role.Value,
                Language = language,
                Oid = oid,
                Avatar = avatar,
                OrganizationName = orgName,
                ChannelOrganization = channelOrganization,
                ParentOrganization = parentOrganization,
                Uid = uid,
                AppKey = appKey
            };
        }

        /// <summary>
        /// Name
        /// 姓名
        /// </summary>
        public required string Name { get; init; }

        private string oid = default!;

        /// <summary>
        /// Organization user Id
        /// 机构用户编号
        /// </summary>
        public required string Oid
        {
            get
            {
                return oid;
            }
            init
            {
                oid = value;
                if (int.TryParse(oid, out var oidValue))
                {
                    OidInt = oidValue;
                }
            }
        }

        /// <summary>
        /// Int organization user id
        /// 整数机构用户编号
        /// </summary>
        public int OidInt { get; init; }

        /// <summary>
        /// Organization name
        /// 机构名称
        /// </summary>
        public string? OrganizationName { get; init; }

        private string? channelOrganization;

        /// <summary>
        /// Channel organization id
        /// 渠道机构编号
        /// </summary>
        public string? ChannelOrganization
        {
            get
            {
                return channelOrganization;
            }
            init
            {
                channelOrganization = value;
                if (int.TryParse(channelOrganization, out var channelOrganizationValue))
                {
                    ChannelOrganizationInt = channelOrganizationValue;
                }
            }
        }

        /// <summary>
        /// Int channel organization id
        /// 整数渠道机构编号
        /// </summary>
        public int? ChannelOrganizationInt { get; init; }

        private string? parentOrganization;

        /// <summary>
        /// Parent organization id
        /// 父机构编号
        /// </summary>
        public string? ParentOrganization
        {
            get
            {
                return parentOrganization;
            }
            init
            {
                parentOrganization = value;
                if (int.TryParse(parentOrganization, out var parentOrganizationValue))
                {
                    ParentOrganizationInt = parentOrganizationValue;
                }
            }
        }

        /// <summary>
        /// Int parent organization id
        /// 整数父机构编号
        /// </summary>
        public int? ParentOrganizationInt { get; init; }

        /// <summary>
        /// Avatar
        /// 头像
        /// </summary>
        public string? Avatar { get; init; }

        private short roleValue;

        /// <summary>
        /// Role value
        /// 角色值
        /// </summary>
        public required short RoleValue
        {
            get
            {
                return roleValue;
            }
            init
            {
                roleValue = value;
                Role = GetRole(roleValue);
            }
        }

        /// <summary>
        /// Role
        /// 角色
        /// </summary>
        public UserRole? Role { get; private set; }

        /// <summary>
        /// Language
        /// 语言
        /// </summary>
        public required CultureInfo Language { get; init; }

        /// <summary>
        /// User uid
        /// 用户uid
        /// </summary>
        public string? Uid { get; init; }

        /// <summary>
        /// App key
        /// 程序键值
        /// </summary>
        public string? AppKey { get; init; }

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

            if (!string.IsNullOrEmpty(Uid))
                claims.Add(new(UidClaim, Uid));

            if (!string.IsNullOrEmpty(AppKey))
                claims.Add(new(AppKeyClaim, AppKey));

            if (Avatar != null)
                claims.Add(new(AvatarClaim, Avatar));

            return claims;
        }
    }
}
