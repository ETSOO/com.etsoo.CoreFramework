using com.etsoo.CoreFramework.Json;
using com.etsoo.Database.Converters;
using System.Net;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// User token
    /// 用户令牌
    /// </summary>
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
        /// Time zone claim type
        /// 时区声明类型
        /// </summary>
        public const string TimeZoneClaim = "timezone";

        /// <summary>
        /// Create refresh token
        /// 创建刷新令牌
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <param name="reason">Failure reason</param>
        /// <returns>Refresh token</returns>
        public new static UserToken? Create(ClaimsPrincipal? claims, out string? reason)
        {
            var user = MinUserToken.Create(claims, out reason);
            if (user == null || claims == null) return null;

            // Claims
            var region = claims.FindFirstValue(RegionClaim);
            var ip = claims.FindFirstValue(IPAddressClaim);
            var deviceId = claims.FindFirstValue(DeviceIdClaim);
            var organization = claims.FindFirstValue(OrganizationClaim);
            var timeZone = claims.FindFirstValue(TimeZoneClaim);

            // Validate
            if (string.IsNullOrEmpty(region))
            {
                reason = "NoRegion";
                return null;
            }

            if (string.IsNullOrEmpty(ip))
            {
                reason = "NoIPAddress";
                return null;
            }

            if (!IPAddress.TryParse(ip, out var ipAddress))
            {
                reason = "InvalidIPAddress:" + ip;
                return null;
            }

            if (string.IsNullOrEmpty(deviceId))
            {
                reason = "NoDeviceId";
                return null;
            }

            if (string.IsNullOrEmpty(organization))
            {
                reason = "NoOrganization";
                return null;
            }

            // New object
            return new UserToken
            {
                Id = user.Id,
                Scopes = user.Scopes,
                RoleValue = user.RoleValue,
                JsonData = user.JsonData,

                ClientIp = ipAddress,
                Region = region,
                DeviceId = deviceId,
                Organization = organization,
                TimeZone = TimeZoneUtils.GetTimeZoneBase(timeZone)
            };
        }

        /// <summary>
        /// Client IP
        /// 客户端IP地址
        /// </summary>
        [JsonConverter(typeof(IPAddressConverter))]
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
        public int DeviceIdInt { get; private init; }

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
        public int OrganizationInt { get; private init; }

        /// <summary>
        /// Time zone
        /// 时区
        /// </summary>
        public TimeZoneInfo? TimeZone { get; init; }

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
                new(DeviceIdClaim, DeviceId),
                new(OrganizationClaim, Organization)
            ]);

            if (TimeZone != null)
            {
                claims.Add(new(TimeZoneClaim, TimeZone.Id));
            }

            return claims;
        }
    }
}
