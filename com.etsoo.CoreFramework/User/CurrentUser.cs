using com.etsoo.CoreFramework.Json;
using com.etsoo.Utils.String;
using System.Globalization;
using System.Net;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Current user data, more information has to be done with additional API call
    /// 当前用户数据，更多信息需要额外的API调用
    /// </summary>
    public record CurrentUser : UserToken, ICurrentUser, IUserCreator<CurrentUser>
    {
        /// <summary>
        /// AppId to permission scope
        /// 程序编号到权限范围
        /// </summary>
        /// <param name="appId">App id</param>
        /// <returns>Scope</returns>
        public static string AppIdToScope(int appId)
        {
            return appId switch
            {
                1 => "core",
                2 => "admin",
                _ => $"app{appId}"
            };
        }

        /// <summary>
        /// Permision scope to app id
        /// 权限范围到程序编号
        /// </summary>
        /// <param name="scope">Scope</param>
        /// <returns>App id</returns>
        public static int ScopeToAppId(string scope)
        {
            return scope switch
            {
                "core" => 1,
                "admin" => 2,
                _ => StringUtils.TryParse<int>(scope[3..]).GetValueOrDefault()
            };
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
        /// Given name claim type
        /// 名声明类型
        /// </summary>
        public const string GivenNameClaim = "givenname";

        /// <summary>
        /// Family name claim type
        /// 姓声明类型
        /// </summary>
        public const string FamilyNameClaim = "familyname";

        /// <summary>
        /// Preferred name claim type
        /// 首选姓名声明类型
        /// </summary>
        public const string PreferredNameClaim = "preferredname";

        /// <summary>
        /// Latin given name claim type
        /// 拉丁名声明类型
        /// </summary>
        public const string LatinGivenNameClaim = "latingivenname";

        /// <summary>
        /// Latin family name claim type
        /// 拉丁姓声明类型
        /// </summary>
        public const string LatinFamilyNameClaim = "latinfamilyname";

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
        /// App claim type
        /// 程序声明类型
        /// </summary>
        public const string AppClaim = "app";

        /// <summary>
        /// Create user
        /// 创建用户
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <param name="reason">Failure reason</param>
        /// <returns>User</returns>
        public new static CurrentUser? Create(ClaimsPrincipal? claims, out string? reason)
        {
            var user = UserToken.Create(claims, out reason);
            if (user == null || claims == null)
            {
                return null;
            }

            // Claims
            var name = claims.FindFirstValue(NameClaim);
            var givenName = claims.FindFirstValue(GivenNameClaim);
            var familyName = claims.FindFirstValue(FamilyNameClaim);
            var preferredName = claims.FindFirstValue(PreferredNameClaim);
            var latinGivenName = claims.FindFirstValue(LatinGivenNameClaim);
            var latinFamilyName = claims.FindFirstValue(LatinFamilyNameClaim);
            var orgName = claims.FindFirstValue(OrganizationNameClaim);
            var avatar = claims.FindFirstValue(AvatarClaim);
            var language = claims.FindFirstValue(LocalityClaim);
            var channelOrganization = claims.FindFirstValue(ChannelOrganizationClaim);
            var parentOrganization = claims.FindFirstValue(ParentOrganizationClaim);
            var oid = StringUtils.TryParse<long>(claims.FindFirstValue(OidClaim));
            var uid = claims.FindFirstValue(UidClaim);
            var app = claims.FindFirstValue(AppClaim);

            // Validate
            if (string.IsNullOrEmpty(name))
            {
                reason = "NoName";
                return null;
            }

            if (string.IsNullOrEmpty(language))
            {
                reason = "NoLanguage";
                return null;
            }

            // New user
            return new CurrentUser
            {
                ClientIp = user.ClientIp,
                DeviceId = user.DeviceId,
                Id = user.Id,
                RoleValue = user.RoleValue,
                Scopes = user.Scopes,
                Region = user.Region,
                Organization = user.Organization,
                TimeZone = user.TimeZone,
                JsonData = user.JsonData,

                Name = name,
                GivenName = givenName,
                FamilyName = familyName,
                PreferredName = preferredName,
                LatinGivenName = latinGivenName,
                LatinFamilyName = latinFamilyName,
                Language = new CultureInfo(language),
                Avatar = avatar,
                OrganizationName = orgName,
                ChannelOrganization = channelOrganization,
                ParentOrganization = parentOrganization,
                Uid = uid,
                Oid = oid.GetValueOrDefault(),
                App = app
            };
        }

        private static (string? id, IEnumerable<string>? scopes, string? organization, short? role, string? deviceId, string? name, string? givenName, string? familyName, string? preferredName, string? latinGivenName, string? latinFamilyName, string? orgName, long? oid, string? avatar, string? channelOrganization, string? parentOrganization, string? uid, string? app) GetData(StringKeyDictionaryObject data)
        {
            return (
                data.Get("Id"),
                data.GetArray("Scopes"),
                data.Get("Organization"),
                data.Get<short>("Role"),
                data.Get("DeviceId"),
                data.Get("Name"),
                data.Get("GivenName"),
                data.Get("FamilyName"),
                data.Get("PreferredName"),
                data.Get("LatinGivenName"),
                data.Get("LatinFamilyName"),
                data.Get("OrgName"),
                data.Get<long>("Oid"),
                data.Get("Avatar"),
                data.Get("ChannelOrganization"),
                data.Get("ParentOrganization"),
                data.Get("Uid"),
                data.Get("App")
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
        public static CurrentUser? Create(StringKeyDictionaryObject data, IPAddress ip, CultureInfo language, string region)
        {
            // Get data
            var (id, scopes, organization, role, deviceId, name, givenName, familyName, preferredName, latinGivenName, latinFamilyName, orgName, oid, avatar, channelOrganization, parentOrganization, uid, app) = GetData(data);

            // Validation
            if (id == null || !role.HasValue || string.IsNullOrEmpty(organization) || string.IsNullOrEmpty(deviceId) || string.IsNullOrEmpty(name))
                return null;

            // New user
            return new CurrentUser
            {
                ClientIp = ip,
                DeviceId = deviceId,
                Id = id,
                Scopes = scopes,
                Region = region,
                Organization = organization,
                RoleValue = role.Value,
                Name = name,
                GivenName = givenName,
                FamilyName = familyName,
                PreferredName = preferredName,
                LatinGivenName = latinGivenName,
                LatinFamilyName = latinFamilyName,
                Language = language,
                Avatar = avatar,
                OrganizationName = orgName,
                ChannelOrganization = channelOrganization,
                ParentOrganization = parentOrganization,
                Uid = uid,
                Oid = oid.GetValueOrDefault(),
                App = app
            };
        }

        /// <summary>
        /// Name
        /// 姓名
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Given name
        /// 名
        /// </summary>
        public string? GivenName { get; init; }

        /// <summary>
        /// Family name
        /// 姓
        /// </summary>
        public string? FamilyName { get; init; }

        /// <summary>
        /// Preferred name
        /// 首选姓名
        /// </summary>
        public string? PreferredName { get; init; }

        /// <summary>
        /// Latin given name
        /// 拉丁名
        /// </summary>
        public string? LatinGivenName { get; init; }

        /// <summary>
        /// Latin family name
        /// 拉丁姓
        /// </summary>
        public string? LatinFamilyName { get; init; }

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
        public int? ChannelOrganizationInt { get; private init; }

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
        public int? ParentOrganizationInt { get; private init; }

        /// <summary>
        /// Avatar
        /// 头像
        /// </summary>
        public string? Avatar { get; init; }

        /// <summary>
        /// Language
        /// 语言
        /// </summary>
        [JsonConverter(typeof(CultureInfoConverter))]
        public required CultureInfo Language { get; init; }

        /// <summary>
        /// User uid
        /// 用户uid
        /// </summary>
        public string? Uid { get; init; }

        /// <summary>
        /// Organization user id, default is 0
        /// 机构用户编号，默认为0
        /// </summary>
        public long Oid { get; init; }

        private string? app;

        /// <summary>
        /// App
        /// 程序
        /// </summary>
        public string? App
        {
            get
            {
                return app;
            }
            init
            {
                app = value;
                if (int.TryParse(app, out var appId))
                {
                    AppId = appId;
                }
            }
        }

        /// <summary>
        /// App id
        /// 程序编号
        /// </summary>
        public int? AppId { get; private init; }

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
                new(OidClaim, Oid.ToString())
            ]);

            if (!string.IsNullOrEmpty(GivenName))
                claims.Add(new(GivenNameClaim, GivenName));

            if (!string.IsNullOrEmpty(FamilyName))
                claims.Add(new(FamilyNameClaim, FamilyName));

            if (!string.IsNullOrEmpty(PreferredName))
                claims.Add(new(PreferredNameClaim, PreferredName));

            if (!string.IsNullOrEmpty(LatinGivenName))
                claims.Add(new(LatinGivenNameClaim, LatinGivenName));

            if (!string.IsNullOrEmpty(LatinFamilyName))
                claims.Add(new(LatinFamilyNameClaim, LatinFamilyName));

            if (!string.IsNullOrEmpty(OrganizationName))
                claims.Add(new(OrganizationNameClaim, OrganizationName));

            if (!string.IsNullOrEmpty(ChannelOrganization))
                claims.Add(new(ChannelOrganizationClaim, ChannelOrganization));

            if (!string.IsNullOrEmpty(ParentOrganization))
                claims.Add(new(ParentOrganizationClaim, ParentOrganization));

            if (!string.IsNullOrEmpty(Uid))
                claims.Add(new(UidClaim, Uid));

            if (!string.IsNullOrEmpty(App))
                claims.Add(new(AppClaim, App));

            if (Avatar != null)
                claims.Add(new(AvatarClaim, Avatar));

            return claims;
        }
    }
}
