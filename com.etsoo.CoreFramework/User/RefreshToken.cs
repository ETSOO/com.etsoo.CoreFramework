using System.Net;
using System.Security.Claims;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Refresh token
    /// 刷新令牌
    /// </summary>
    public record RefreshToken : UserToken, IRefreshToken
    {
        /// <summary>
        /// Create refresh token
        /// 创建刷新令牌
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <returns>Refresh token</returns>
        public static new RefreshToken? Create(ClaimsPrincipal claims)
        {
            var token = UserToken.Create(claims);
            if (token == null) return null;

            var sid = claims.FindFirstValue(ClaimTypes.PrimarySid);

            return new RefreshToken(token.Id, token.Organization, token.ClientIp, token.Region, token.DeviceId, sid);
        }

        /// <summary>
        /// Service identifier
        /// 服务识别号
        /// </summary>
        public string? Sid { get; set; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="id">User id</param>
        /// <param name="organization">Organization id</param>
        /// <param name="clientIp"></param>
        /// <param name="region">Country or region</param>
        /// <param name="deviceId">Device id</param>
        /// <param name="sid">Service identifier id</param>
        public RefreshToken(string id, string? organization, IPAddress clientIp, string region, int deviceId, string? sid = null)
            : base(id, clientIp, region, deviceId, organization)
        {
            Sid = sid;
        }

        /// <summary>
        /// Create claims
        /// 创建声明
        /// </summary>
        /// <returns>Claims</returns>
        public override IEnumerable<Claim> MoreClaims()
        {
            if (!string.IsNullOrEmpty(Sid))
                yield return new(ClaimTypes.PrimarySid, Sid);
        }
    }
}
