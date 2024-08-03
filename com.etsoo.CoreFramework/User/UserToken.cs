using System.Net;
using System.Security.Claims;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// User token
    /// 用户令牌
    /// </summary>
    public record UserToken : MinUserToken, IUserToken
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
        /// Device id claim type
        /// 设备编号声明类型
        /// </summary>
        public const string DeviceIdClaim = "deviceid";

        /// <summary>
        /// Create refresh token
        /// 创建刷新令牌
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <param name="connectionId">Connection id</param>
        /// <returns>Refresh token</returns>
        public new static UserToken? Create(ClaimsPrincipal? claims, string? connectionId = null)
        {
            if (claims == null) return null;

            var user = MinUserToken.Create(claims, connectionId);
            if (user == null) return null;

            // Claims
            var region = claims.FindFirstValue(ClaimTypes.Country);
            var ip = claims.FindFirstValue(IPAddressClaim);
            var deviceId = claims.FindFirstValue(DeviceIdClaim);
            var organization = claims.FindFirstValue(OrganizationClaim);

            // Validate
            if (string.IsNullOrEmpty(region)
                || string.IsNullOrEmpty(ip)
                || !IPAddress.TryParse(ip, out var ipAddress)
                || string.IsNullOrEmpty(deviceId))
                return null;

            // New object
            return new UserToken(user.Id, user.Scopes, ipAddress, region, deviceId, organization, user.ConnectionId);
        }

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
        public string DeviceId { get; }

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
        /// <param name="scopes">Scopes</param>
        /// <param name="clientIp"></param>
        /// <param name="region">Country or region</param>
        /// <param name="deviceId">Device id</param>
        /// <param name="organization">Organization</param>
        /// <param name="connectionId">Connection id</param>
        public UserToken(string id, IEnumerable<string>? scopes, IPAddress clientIp, string region, string deviceId, string? organization, string? connectionId = null)
            : base(id, scopes, connectionId)
        {
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
        /// Create claims
        /// 创建声明
        /// </summary>
        /// <returns>Claims</returns>
        protected override List<Claim> CreateClaims()
        {
            var claims = base.CreateClaims();

            claims.AddRange([
                new(ClaimTypes.Country, Region),
                new(IPAddressClaim, ClientIp.ToString()),
                new(DeviceIdClaim, DeviceId)
            ]);

            if (!string.IsNullOrEmpty(Organization))
                claims.Add(new(OrganizationClaim, Organization));

            return claims;
        }
    }
}
