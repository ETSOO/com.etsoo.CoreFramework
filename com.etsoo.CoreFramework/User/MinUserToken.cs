using System.Security.Claims;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// Minimal user token
    /// 最小用户令牌
    /// </summary>
    public record MinUserToken : IMinUserToken, IMinUserCreator<MinUserToken>
    {
        /// <summary>
        /// Id claim type
        /// 编号声明类型
        /// </summary>
        public const string IdClaim = "nameid";

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
            var id = claims.FindFirstValue(IdClaim);
            var scopes = claims.FindAll(ScopeClaim).Select(claim => claim.Value);

            // Validate
            if (string.IsNullOrEmpty(id))
                return null;

            // New object
            return new MinUserToken
            {
                Id = id,
                ConnectionId = connectionId,
                Scopes = scopes
            };
        }

        string id = default!;

        /// <summary>
        /// User Id
        /// 用户编号
        /// </summary>
        public required string Id
        {
            get
            {
                return id;
            }
            init
            {
                id = value;
                if (int.TryParse(id, out var idValue))
                {
                    IdInt = idValue;
                }
            }
        }

        /// <summary>
        /// Int id
        /// 整数编号
        /// </summary>
        public int IdInt { get; init; }

        /// <summary>
        /// Connection id
        /// 链接编号
        /// </summary>
        public string? ConnectionId { get; init; }

        /// <summary>
        /// Scopes
        /// 范围
        /// </summary>
        public IEnumerable<string>? Scopes { get; init; }

        /// <summary>
        /// Create claims
        /// 创建声明
        /// </summary>
        /// <returns>Claims</returns>
        protected virtual List<Claim> CreateClaims()
        {
            var claims = new List<Claim>
            {
                new(IdClaim, Id)
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
