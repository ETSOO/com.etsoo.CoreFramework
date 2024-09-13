using System.Net;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// User token
    /// 用户令牌
    /// </summary>
    [JsonDerivedType(typeof(CurrentUser))]
    public record UserToken : MinUserToken, IUserToken
    {
        /// <summary>
        /// Region claim type
        /// 区域声明类型
        /// </summary>
        public const string RegionClaim = "region";

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
            var region = claims.FindFirstValue(RegionClaim);
            var ip = claims.FindFirstValue(IPAddressClaim);
            var deviceId = claims.FindFirstValue(DeviceIdClaim);
            var organization = claims.FindFirstValue(OrganizationClaim);

            // Validate
            if (string.IsNullOrEmpty(region)
                || string.IsNullOrEmpty(ip)
                || !IPAddress.TryParse(ip, out var ipAddress)
                || string.IsNullOrEmpty(deviceId)
                || string.IsNullOrEmpty(organization))
                return null;

            // New object
            return new UserToken
            {
                Id = user.Id,
                ConnectionId = user.ConnectionId,
                Scopes = user.Scopes,

                ClientIp = ipAddress,
                Region = region,
                DeviceId = deviceId,
                Organization = organization
            };
        }

        /// <summary>
        /// Client IP
        /// 客户端IP地址
        /// </summary>
        public required IPAddress ClientIp { get; init; }

        /// <summary>
        /// Country or region, like CN means China
        /// 国家和地区，比如 CN = 中国
        /// </summary>
        public required string Region { get; init; }

        private string deviceId = default!;

        /// <summary>
        /// Device id
        /// 设备编号
        /// </summary>
        public required string DeviceId
        {
            get { return deviceId; }
            init
            {
                deviceId = value;
                if (int.TryParse(deviceId, out var deviceValue))
                {
                    DeviceIdInt = deviceValue;
                }
            }
        }

        /// <summary>
        /// Int device id
        /// 整数设备编号
        /// </summary>
        public int DeviceIdInt { get; init; }

        private string organization = default!;

        /// <summary>
        /// Organization id, support switch
        /// 机构编号，可切换
        /// </summary>
        public required string Organization
        {
            get { return organization; }
            init
            {
                organization = value;
                if (int.TryParse(organization, out var organizationValue))
                {
                    OrganizationInt = organizationValue;
                }
            }
        }

        /// <summary>
        /// Int organization id, default 0
        /// 整数机构编号，默认为0
        /// </summary>
        public int OrganizationInt { get; init; }

        /// <summary>
        /// Create claims
        /// 创建声明
        /// </summary>
        /// <returns>Claims</returns>
        protected override List<Claim> CreateClaims()
        {
            var claims = base.CreateClaims();

            claims.AddRange([
                new(RegionClaim, Region),
                new(IPAddressClaim, ClientIp.ToString()),
                new(DeviceIdClaim, DeviceId)
            ]);

            if (!string.IsNullOrEmpty(Organization))
                claims.Add(new(OrganizationClaim, Organization));

            return claims;
        }
    }
}
