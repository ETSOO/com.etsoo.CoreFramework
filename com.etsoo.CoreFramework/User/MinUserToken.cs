using System.Security.Claims;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Minimal user token
    /// 最小用户令牌
    /// </summary>
    public record MinUserToken : IMinUserToken
    {
        /// <summary>
        /// Scope claim type
        /// 范围声明类型
        /// </summary>
        public const string ScopeClaim = "scope";

        /// <summary>
        /// Create refresh token
        /// 创建刷新令牌
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <param name="connectionId">Connection id</param>
        /// <returns>Refresh token</returns>
        public static MinUserToken? Create(ClaimsPrincipal? claims, string? connectionId = null)
        {
            // Basic check
            if (claims == null || claims.Identity == null || !claims.Identity.IsAuthenticated)
                return null;

            // Claims
            var id = claims.FindFirstValue(ClaimTypes.NameIdentifier);
            var scopes = claims.FindAll(ScopeClaim).Select(claim => claim.Value);

            // Validate
            if (string.IsNullOrEmpty(id))
                return null;

            // New object
            return new MinUserToken(id, scopes, connectionId);
        }

        /// <summary>
        /// User Id
        /// 用户编号
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Connection id
        /// 链接编号
        /// </summary>
        public string? ConnectionId { get; }

        /// <summary>
        /// Scopes
        /// 范围
        /// </summary>
        public IEnumerable<string>? Scopes { get; protected set; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="id">User id</param>
        /// <param name="scopes">Scopes</param>
        /// <param name="connectionId">Connection id</param>
        public MinUserToken(string id, IEnumerable<string>? scopes = null, string? connectionId = null)
        {
            Id = id;
            Scopes = scopes;
            ConnectionId = connectionId;
        }

        /// <summary>
        /// Create claims
        /// 创建声明
        /// </summary>
        /// <returns>Claims</returns>
        protected virtual List<Claim> CreateClaims()
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, Id)
            };

            if (Scopes != null)
            {
                claims.AddRange(Scopes.Select(scope => new Claim(ScopeClaim, scope)));
            }

            return claims;
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
