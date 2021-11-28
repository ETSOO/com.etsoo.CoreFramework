using com.etsoo.Utils.String;
using System.Globalization;
using System.Net;
using System.Security.Claims;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Service user data
    /// 服务用户数据
    /// </summary>
    public record ServiceUser : UserToken, IServiceUser
    {
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
        /// <returns>User</returns>
        public new static ServiceUser? Create(ClaimsPrincipal? claims)
        {
            var token = UserToken.Create(claims);
            if (token == null) return null;

            // Claims
            var organization = claims.FindFirstValue(OrganizationClaim);
            var language = claims.FindFirstValue(ClaimTypes.Locality);
            var roleValue = StringUtils.TryParse<short>(claims.FindFirstValue(RoleValueClaim)).GetValueOrDefault();

            // Validate
            if (string.IsNullOrEmpty(language))
                return null;

            // New user
            return new ServiceUser(
                token.Id,
                organization,
                roleValue,
                token.ClientIp,
                token.DeviceId,
                new CultureInfo(language),
                token.Region);
        }

        /// <summary>
        /// Get dictionary data
        /// 获取字典数据
        /// </summary>
        /// <param name="data">Input</param>
        /// <returns>Result</returns>
        protected static (string? id, string? Organization, short? Role, int? deviceId) GetData(StringKeyDictionaryObject data)
        {
            return (
                data.Get("Id"),
                data.Get("Organization"),
                data.Get<short>("Role"),
                data.Get<int>("DeviceId")
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
        public static ServiceUser? Create(StringKeyDictionaryObject data, IPAddress ip, CultureInfo language, string region)
        {
            // Get data
            var (id, organization, role, deviceId) = GetData(data);

            // Validation
            if (id == null)
                return null;

            // New user
            return new ServiceUser(
                id,
                organization,
                role.GetValueOrDefault(),
                ip,
                deviceId.GetValueOrDefault(),
                language,
                region);
        }

        /// <summary>
        /// Organization id, support switch
        /// 机构编号，可切换
        /// </summary>
        public string? Organization { get; private set; }

        /// <summary>
        /// Int id
        /// 整数编号
        /// </summary>
        public int IdInt { get; }

        /// <summary>
        /// Int organization id
        /// 整数机构编号
        /// </summary>
        public int? OrganizationInt { get; }

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
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="organization">Organization</param>
        /// <param name="roleValue">Role value</param>
        /// <param name="clientIp">Client IP</param>
        /// <param name="deviceId">Device id</param>
        /// <param name="language">Language</param>
        /// <param name="region">Country or region</param>
        public ServiceUser(string id, string? organization, short roleValue, IPAddress clientIp, int deviceId, CultureInfo language, string region)
            :base(id, clientIp, region, deviceId)
        {
            Organization = organization;
            RoleValue = roleValue;
            Language = language;

            if (int.TryParse(Id, out var idValue))
            {
                IdInt = idValue;
            }

            if (int.TryParse(Organization, out var organizationValue))
            {
                OrganizationInt = organizationValue;
            }
        }

        private IEnumerable<Claim> GetClaims()
        {
            yield return new(ClaimTypes.Locality, Language.Name);
            yield return new(RoleValueClaim, RoleValue.ToString());

            if (!string.IsNullOrEmpty(Organization))
                yield return new(OrganizationClaim, Organization);
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
            var (_, organization, role, _) = GetData(data);

            // Organization
            Organization = organization;

            // Role
            if (role != null && RoleValue != role)
                RoleValue = role.Value;
        }
    }
}
