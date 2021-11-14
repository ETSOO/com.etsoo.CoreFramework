using System.Net;
using System.Security.Claims;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Refresh token
    /// 刷新令牌
    /// </summary>
    public record RefreshToken : IRefreshToken
    {
        /// <summary>
        /// IP Address claim type
        /// IP地址声明类型
        /// </summary>
        public const string IPAddressClaim = "ipaddress";

        /// <summary>
        /// Create refresh token
        /// 创建刷新令牌
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <returns>Refresh token</returns>
        public static RefreshToken? Create(ClaimsPrincipal? claims)
        {
            // Basic check
            if (claims == null || claims.Identity == null || !claims.Identity.IsAuthenticated)
                return null;

            // Claims
            var id = claims.FindFirstValue(ClaimTypes.NameIdentifier);
            var region = claims.FindFirstValue(ClaimTypes.Country);
            var ip = claims.FindFirstValue(IPAddressClaim);
            var sid = claims.FindFirstValue(ClaimTypes.PrimarySid);

            // Validate
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(region) || string.IsNullOrEmpty(ip) || !IPAddress.TryParse(ip, out var ipAddress) || string.IsNullOrEmpty(sid))
                return null;

            // New object
            return new RefreshToken(id, ipAddress, region, sid);
        }

        /// <summary>
        /// User Id
        /// 用户编号
        /// </summary>
        public string Id { get; }

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
        /// Service identifier
        /// 服务识别号
        /// </summary>
        public string Sid { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="id">User id</param>
        /// <param name="clientIp"></param>
        /// <param name="region">Country or region</param>
        /// <param name="sid">Service identifier id</param>
        public RefreshToken(string id, IPAddress clientIp, string region, string sid)
        {
            Id = id;
            ClientIp = clientIp;
            Region = region;
            Sid = sid;
        }

        /// <summary>
        /// Create claims
        /// 创建声明
        /// </summary>
        /// <returns>Claims</returns>
        public virtual IEnumerable<Claim> CreateClaims()
        {
            yield return new(ClaimTypes.NameIdentifier, Id);
            yield return new(ClaimTypes.Country, Region);
            yield return new(IPAddressClaim, ClientIp.ToString());
            yield return new(ClaimTypes.PrimarySid, Sid);
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
