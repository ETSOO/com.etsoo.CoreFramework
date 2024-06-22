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
        public static CurrentUser? Create(ClaimsPrincipal? claims, string? connectionId = null)
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

        private static (string? id, string? organization, short? Role, int? deviceId, string? Name, string? OrgName, string? Avatar) GetData(StringKeyDictionaryObject data)
        {
            return (
                data.Get("Id"),
                data.Get("Organization"),
                data.Get<short>("Role"),
                data.Get<int>("DeviceId"),
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
            var (id, organization, role, deviceId, name, orgName, avatar) = GetData(data);

            // Validation
            if (id == null || role == null || deviceId == null || string.IsNullOrEmpty(name))
                return null;

            // New user
            return new CurrentUser(
                id,
                organization,
                name,
                role.Value,
                ip,
                deviceId.Value,
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
        /// Connection id
        /// 链接编号
        /// </summary>
        public string? ConnectionId { get; }

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
        /// <param name="connectionId">Connection id</param>
        public CurrentUser(string id, string? organization, string name, short roleValue, IPAddress clientIp, int deviceId, CultureInfo language, string region, string? connectionId = null)
            : base(id, clientIp, region, deviceId, organization)
        {
            Name = name;
            RoleValue = roleValue;
            Role = ServiceUser.GetRole(roleValue);

            Language = language;

            ConnectionId = connectionId;
        }

        /// <summary>
        /// Create claims
        /// 创建声明
        /// </summary>
        /// <returns>Claims</returns>
        public override IEnumerable<Claim> MoreClaims()
        {
            yield return new(ClaimTypes.Name, Name);

            if (!string.IsNullOrEmpty(OrganizationName))
                yield return new(OrganizationNameClaim, OrganizationName);

            if (Avatar != null)
                yield return new(AvatarClaim, Avatar);

            yield return new(ClaimTypes.Locality, Language.Name);
            yield return new(ServiceUser.RoleValueClaim, RoleValue.ToString());
        }

        /// <summary>
        /// Update
        /// 更新
        /// </summary>
        /// <param name="data">Data collection</param>
        public virtual void Update(StringKeyDictionaryObject data)
        {
            // Editable fields
            var (_, _, role, _, name, orgName, avatar) = GetData(data);

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
