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
    public record CurrentUser : UserToken, ICurrentUser
    {
        /// <summary>
        /// Avatar claim type
        /// 头像声明类型
        /// </summary>
        public const string AvatarClaim = "avatar";

        /// <summary>
        /// Organization claim type
        /// 机构声明类型
        /// </summary>
        public const string OrganizationClaim = "organization";

        /// <summary>
        /// Role value claim type
        /// 角色值类型
        /// </summary>
        public const string RoleValueClaim = "rolevalue";

        /// <summary>
        /// Create user
        /// 创建用户
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <param name="connectionId">Connection id</param>
        /// <returns>User</returns>
        public static CurrentUser? Create(ClaimsPrincipal? claims, string? connectionId = null)
        {
            var token = UserToken.Create(claims);
            if (token == null) return null;

            // Claims
            var name = claims.FindFirstValue(ClaimTypes.Name);
            var avatar = claims.FindFirstValue(AvatarClaim);
            var organization = claims.FindFirstValue(OrganizationClaim);
            var language = claims.FindFirstValue(ClaimTypes.Locality);
            var roleValue = StringUtils.TryParse<short>(claims.FindFirstValue(RoleValueClaim)).GetValueOrDefault();

            // Universal id
            var uidText = claims.FindFirstValue(ClaimTypes.Upn);
            Guid? uid = Guid.TryParse(uidText, out var uidExact) ? uidExact : null;

            // Validate
            if (string.IsNullOrEmpty(name)
                || string.IsNullOrEmpty(language))
                return null;

            // New user
            return new CurrentUser(
                token.Id,
                organization,
                name, roleValue,
                token.ClientIp,
                token.DeviceId,
                new CultureInfo(language),
                token.Region,
                uid,
                connectionId)
            {
                Avatar = avatar
            };
        }

        private static (string? Organization, string? Name, short? Role, string? Avatar, int? deviceId, string? JsonData) GetData(StringKeyDictionaryObject data)
        {
            return (
                data.Get("Organization"),
                data.Get("Name"),
                data.Get<short>("Role"),
                data.Get("Avatar"),
                data.Get<int>("DeviceId"),
                data.Get("JsonData")
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
        /// <returns>User</returns>
        public static CurrentUser? Create(StringKeyDictionaryObject data, IPAddress ip, CultureInfo language, string region, Guid? uid = null, string? connectionId = null)
        {
            // Get data
            var id = data.Get("Id");
            var (organization, name, role, avatar, deviceId, jsonData) = GetData(data);

            // Validation
            if (id == null || string.IsNullOrEmpty(name))
                return null;

            // New user
            return new CurrentUser(
                id,
                organization,
                name,
                role.GetValueOrDefault(),
                ip,
                deviceId.GetValueOrDefault(),
                language,
                region,
                uid,
                connectionId)
            {
                Avatar = avatar,
                JsonData = jsonData
            };
        }

        /// <summary>
        /// Organization id, support switch
        /// 机构编号，可切换
        /// </summary>
        public string? Organization { get; private set; }

        /// <summary>
        /// Name
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Role value
        /// 角色值
        /// </summary>
        public virtual short RoleValue { get; private set; }

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
        public string? Avatar { get; private set; }

        /// <summary>
        /// Json data
        /// Json数据
        /// </summary>
        public string? JsonData { get; set; }

        /// <summary>
        /// Universal id
        /// 全局编号
        /// </summary>
        public Guid? Uid { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="organization">Organization</param>
        /// <param name="name">Name</param>
        /// <param name="roleValue">Role value</param>
        /// <param name="clientIp">Client IP</param>
        /// <param name="deviceId">Device id</param>
        /// <param name="language">Language</param>
        /// <param name="region">Country or region</param>
        /// <param name="uid">Universal id</param>
        /// <param name="connectionId">Connection id</param>
        public CurrentUser(string id, string? organization, string name, short roleValue, IPAddress clientIp, int deviceId, CultureInfo language, string region, Guid? uid, string? connectionId)
            :base(id, clientIp, region, deviceId)
        {
            Organization = organization;
            Name = name;
            RoleValue = roleValue;
            Language = language;
            Uid = uid;
            ConnectionId = connectionId;
        }

        private IEnumerable<Claim> GetClaims()
        {
            yield return new(ClaimTypes.Name, Name);
            yield return new(ClaimTypes.Locality, Language.Name);
            yield return new(RoleValueClaim, RoleValue.ToString());

            if (!string.IsNullOrEmpty(Organization))
                yield return new(OrganizationClaim, Organization);
            if (Avatar != null)
                yield return new(AvatarClaim, Avatar);
            if (!string.IsNullOrEmpty(JsonData))
                yield return new(ClaimTypes.UserData, JsonData);
            if (Uid != null)
                yield return new(ClaimTypes.Upn, Uid.Value.ToString());
        }

        /// <summary>
        /// Create claims
        /// 创建声明
        /// </summary>
        /// <returns>Claims</returns>
        public override IEnumerable<Claim> CreateClaims()
        {
            var claims = GetClaims();
            return base.CreateClaims().Concat(claims);
        }

        /// <summary>
        /// Update
        /// 更新
        /// </summary>
        /// <param name="data">Data collection</param>
        public virtual void Update(StringKeyDictionaryObject data)
        {
            // Editable fields
            var (organization, name, role, avatar, _, jsonData) = GetData(data);

            // Name
            if (name != null)
                Name = name;

            // Avatar
            Avatar = avatar;

            // Organization
            Organization = organization;

            // Json data
            JsonData = jsonData;

            // Role
            if (role != null && RoleValue != role)
                RoleValue = role.Value;
        }
    }
}
