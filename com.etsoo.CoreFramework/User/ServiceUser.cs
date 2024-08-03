using com.etsoo.CoreFramework.Authentication;
using com.etsoo.Utils;
using com.etsoo.Utils.String;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Security.Claims;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Service user data, more information has to be done with additional API call
    /// 服务用户数据，更多信息需要额外的API调用
    /// </summary>
    public record ServiceUser : UserToken, IServiceUser, IUserCreator<ServiceUser>
    {
        /// <summary>
        /// Role value claim type
        /// 角色值类型
        /// </summary>
        public const string RoleValueClaim = "rolevalue";

        /// <summary>
        /// Service claim type
        /// 服务值类型
        /// </summary>
        public const string ServiceClaim = "service";

        /// <summary>
        /// Create user from claims
        /// 从声明创建用户
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <param name="connectionId">Connection id</param>
        /// <returns>User</returns>
        public new static ServiceUser? Create(ClaimsPrincipal? claims, string? connectionId = null)
        {
            if (claims == null) return null;

            var token = UserToken.Create(claims);
            if (token == null) return null;

            // Claims
            var language = claims.FindFirstValue(ClaimTypes.Locality);
            var roleValue = StringUtils.TryParse<short>(claims.FindFirstValue(RoleValueClaim)).GetValueOrDefault();
            var userUidString = claims.FindFirstValue(ClaimTypes.PrimarySid);

            // Validate
            if (string.IsNullOrEmpty(language))
                return null;

            // Service
            var service = claims.FindFirstValue(ServiceClaim);

            // New user
            return new ServiceUser(
                token.Id,
                token.Scopes,
                userUidString,
                token.Organization,
                roleValue,
                token.ClientIp,
                token.DeviceId,
                new CultureInfo(language),
                token.Region,
                connectionId)
            {
                Service = service
            };
        }

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
        /// Get dictionary data
        /// 获取字典数据
        /// </summary>
        /// <param name="data">Input</param>
        /// <returns>Result</returns>
        static (string? id, IEnumerable<string>? scopes, string? organization, short? Role, string? deviceId, string? uid) GetData(StringKeyDictionaryObject data)
        {
            return (
                data.Get("Id"),
                data.GetArray("Scopes"),
                data.Get("Organization"),
                data.Get<short>("Role"),
                data.Get("DeviceId"),
                data.Get("Uid")
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
        public static ServiceUser? Create(StringKeyDictionaryObject data, IPAddress ip, CultureInfo language, string region, string? connectionId = null)
        {
            // Get data
            var (id, scopes, organization, role, deviceId, uid) = GetData(data);

            // Validation
            if (id == null || role == null || string.IsNullOrEmpty(deviceId))
                return null;

            // New user
            return new ServiceUser(
                id,
                scopes,
                uid,
                organization,
                role.Value,
                ip,
                deviceId,
                language,
                region,
                connectionId);
        }

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
        /// Service
        /// 服务
        /// </summary>
        public string? Service { get; set; }

        /// <summary>
        /// User Uid
        /// 用户全局编号
        /// </summary>
        [NotNullIfNotNull(nameof(Organization))]
        public string? Uid { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="scopes">Scopes</param>
        /// <param name="uid">Uid, shared between multiple applications</param>
        /// <param name="organization">Organization identifying units in SaSS</param>
        /// <param name="roleValue">Role value</param>
        /// <param name="clientIp">Client IP</param>
        /// <param name="deviceId">Device id</param>
        /// <param name="language">Language</param>
        /// <param name="region">Country or region</param>
        /// <param name="connectionId">Connection id</param>
        public ServiceUser(string id, IEnumerable<string>? scopes, string? uid, string? organization, short roleValue, IPAddress clientIp, string deviceId, CultureInfo language, string region, string? connectionId = null)
            : base(id, scopes, clientIp, region, deviceId, organization)
        {
            RoleValue = roleValue;
            Role = GetRole(roleValue);
            Language = language;
            Uid = uid;
        }

        /// <summary>
        /// Constructor, for simple case
        /// 构造函数，适应于简单情形
        /// </summary>
        /// <param name="role">User role</param>
        /// <param name="id">Username</param>
        /// <param name="scopes">Scopes</param>
        /// <param name="clientIp">Client IP</param>
        /// <param name="language">Language</param>
        /// <param name="region">Country or region</param>
        /// <param name="organization">Organization</param>
        /// <param name="uid">Uid, shared between multiple applications</param>
        /// <param name="deviceId">Device id</param>
        /// <param name="connectionId">Connection id</param>
        public ServiceUser(UserRole role, string id, IEnumerable<string>? scopes, IPAddress clientIp, CultureInfo language, string region, string? organization = null, string? uid = null, string deviceId = "", string? connectionId = null)
            : this(id, scopes, uid, organization, (short)role, clientIp, deviceId, language, region, connectionId)
        {
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
                new(ClaimTypes.Locality, Language.Name),
                new(RoleValueClaim, RoleValue.ToString())
            ]);

            if (Uid != null) claims.Add(new(ClaimTypes.PrimarySid, Uid));
            if (Service != null) claims.Add(new(ServiceClaim, Service));

            if (Role.HasValue)
            {
                var roles = Role.Value.GetKeys();
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            }

            return claims;
        }

        /// <summary>
        /// Update role
        /// 更新角色
        /// </summary>
        /// <param name="newRole">New role</param>
        public void Update(UserRole newRole)
        {
            if (!Role.Equals(newRole))
            {
                Role = newRole;
                RoleValue = (short)newRole;
            }
        }

        /// <summary>
        /// Update
        /// 更新
        /// </summary>
        /// <param name="data">Data collection</param>
        public virtual void Update(StringKeyDictionaryObject data)
        {
            // Editable fields
            var (_, scopes, _, role, _, _) = GetData(data);

            // Scopes
            Scopes = scopes;

            // Role
            if (role != null && RoleValue != role)
                RoleValue = role.Value;
        }
    }
}
