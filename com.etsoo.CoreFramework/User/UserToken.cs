using System.Net;
using System.Security.Claims;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// User token
    /// 用户令牌
    /// </summary>
    public record UserToken : IUserToken
    {
        /// <summary>
        /// IP Address claim type
        /// IP地址声明类型
        /// </summary>
        public const string IPAddressClaim = "ipaddress";

        /// <summary>
        /// Organization claim type
        /// 机构声明类型
        /// </summary>
        public const string OrganizationClaim = "organization";

        /// <summary>
        /// Create refresh token
        /// 创建刷新令牌
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <returns>Refresh token</returns>
        public static UserToken? Create(ClaimsPrincipal? claims)
        {
            // Basic check
            if (claims == null || claims.Identity == null || !claims.Identity.IsAuthenticated)
                return null;

            // Claims
            var id = claims.FindFirstValue(ClaimTypes.NameIdentifier);
            var region = claims.FindFirstValue(ClaimTypes.Country);
            var ip = claims.FindFirstValue(IPAddressClaim);
            var device = claims.FindFirstValue(ClaimTypes.AuthenticationInstant);
            var organization = claims.FindFirstValue(OrganizationClaim);

            // Validate
            if (string.IsNullOrEmpty(id)
                || string.IsNullOrEmpty(region)
                || string.IsNullOrEmpty(ip)
                || !IPAddress.TryParse(ip, out var ipAddress)
                || string.IsNullOrEmpty(device)
                || !int.TryParse(device, out var deviceId))
                return null;

            // New object
            return new UserToken(id, ipAddress, region, deviceId, organization);
        }

        /// <summary>
        /// User Id
        /// 用户编号
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Int id
        /// 整数编号
        /// </summary>
        public int IdInt { get; }

        /// <summary>
        /// Client IP
        /// 客户端IP地址
        /// </summary>
        public IPAddress ClientIp { get; }

        /// <summary>
        /// Country or region, like CN means China
        /// 国家和地区，比如 CN = 中国
        /// </summary>
        public string Region { get; }

        /// <summary>
        /// Device id
        /// 设备编号
        /// </summary>
        public int DeviceId { get; }

        /// <summary>
        /// Organization id, support switch
        /// 机构编号，可切换
        /// </summary>
        public string? Organization { get; private set; }

        /// <summary>
        /// Int organization id
        /// 整数机构编号
        /// </summary>
        public int? OrganizationInt { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="id">User id</param>
        /// <param name="clientIp"></param>
        /// <param name="region">Country or region</param>
        /// <param name="deviceId">Device id</param>
        /// <param name="organization">Organization</param>
        public UserToken(string id, IPAddress clientIp, string region, int deviceId, string? organization)
        {
            Id = id;
            ClientIp = clientIp;
            Region = region;
            DeviceId = deviceId;
            Organization = organization;

            if (int.TryParse(id, out var idValue))
            {
                IdInt = idValue;
            }

            if (int.TryParse(organization, out var organizationValue))
            {
                OrganizationInt = organizationValue;
            }
        }

        /// <summary>
        /// More claims to provide
        /// 提供更多声明
        /// </summary>
        /// <returns>Claims</returns>
        public virtual IEnumerable<Claim> MoreClaims()
        {
            yield break;
        }

        /// <summary>
        /// Create claims
        /// 创建声明
        /// </summary>
        /// <returns>Claims</returns>
        protected IEnumerable<Claim> CreateClaims()
        {
            yield return new(ClaimTypes.NameIdentifier, Id);
            yield return new(ClaimTypes.Country, Region);
            yield return new(IPAddressClaim, ClientIp.ToString());
            yield return new(ClaimTypes.AuthenticationInstant, DeviceId.ToString());

            if (!string.IsNullOrEmpty(Organization))
                yield return new(OrganizationClaim, Organization);

            foreach (var claim in MoreClaims())
            {
                yield return claim;
            }
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
