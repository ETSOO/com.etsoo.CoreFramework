using System.Collections.Generic;
using System.Net;
using System.Security.Claims;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Current user data
    /// 当前用户数据
    /// </summary>
    public abstract record CurrentUser(string Id, string Name, string Role, IPAddress ClientIp, string Language, string? ConnectionId) : ICurrentUser
    {
        /// <summary>
        /// IP Address claim type
        /// IP地址声明类型
        /// </summary>
        public const string IPAddress = "ipaddress";

        /// <summary>
        /// Create claims
        /// 创建声明
        /// </summary>
        /// <returns>Claims</returns>
        public virtual IEnumerable<Claim> CreateClaims()
        {
            return new Claim[] {
                new (ClaimTypes.Name, Name),
                new (ClaimTypes.NameIdentifier, Id),
                new (ClaimTypes.Locality, Language),
                new (ClaimTypes.Role, Role),
                new (IPAddress, ClientIp.ToString())
            };
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
