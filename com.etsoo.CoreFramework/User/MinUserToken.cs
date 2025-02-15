using com.etsoo.CoreFramework.Authentication;
using com.etsoo.Utils;
using com.etsoo.Utils.Serialization;
using com.etsoo.Utils.String;
using System.Security.Claims;
using System.Text.Json;

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
        /// JSON data claim type
        /// JSON数据声明类型
        /// </summary>
        public const string JsonDataClaim = "jsondata";

        /// <summary>
        /// Role value claim type
        /// 角色值声明类型
        /// </summary>
        public const string RoleValueClaim = "role";

        /// <summary>
        /// Create refresh token
        /// 创建刷新令牌
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <param name="reason">Failure reason</param>
        /// <returns>Refresh token</returns>
        public static MinUserToken? Create(ClaimsPrincipal? claims, out string? reason)
        {
            // Basic check
            if (claims == null || claims.Identity == null)
            {
                // Ignore the reason
                reason = null;
                return null;
            }

            if (!claims.Identity.IsAuthenticated)
            {
                reason = "NotAuthenticated";
                return null;
            }

            // Claims
            var id = claims.FindFirstValue(IdClaim);
            var scopes = claims.FindAll(ScopeClaim).Select(claim => claim.Value);
            var jsonData = claims.FindFirstValue(JsonDataClaim);
            var roleValue = StringUtils.TryParse<short>(claims.FindFirstValue(RoleValueClaim)).GetValueOrDefault((short)UserRole.Guest);

            // Validate
            if (string.IsNullOrEmpty(id))
            {
                reason = "NoId";
                return null;
            }

            var data = string.IsNullOrEmpty(jsonData) ? null : JsonSerializer.Deserialize(jsonData, CommonJsonSerializerContext.Default.DictionaryStringObject);

            // Success
            reason = null;

            // New object
            return new MinUserToken
            {
                Id = id,
                Scopes = scopes,
                JsonData = data == null ? [] : new StringKeyDictionaryObject(data!),
                RoleValue = roleValue
            };
        }

        /// <summary>
        /// Get role from value
        /// 从值获取角色
        /// </summary>
        /// <param name="roleValue">Role value</param>
        /// <returns>User role</returns>
        public static UserRole? GetRole(short roleValue)
        {
            var userRole = (UserRole)roleValue;
            return SharedUtils.EnumIsDefined(userRole) ? userRole : null;
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
        /// Scopes
        /// 范围
        /// </summary>
        public IEnumerable<string>? Scopes { get; init; }

        /// <summary>
        /// JSON data
        /// JSON数据
        /// </summary>
        public StringKeyDictionaryObject JsonData { get; init; } = [];

        private short roleValue;

        /// <summary>
        /// Role value
        /// 角色值
        /// </summary>
        public short RoleValue
        {
            get
            {
                return roleValue;
            }
            init
            {
                roleValue = value;
                Role = GetRole(roleValue);
            }
        }

        /// <summary>
        /// Role
        /// 角色
        /// </summary>
        public UserRole? Role { get; private set; }

        /// <summary>
        /// Create claims
        /// 创建声明
        /// </summary>
        /// <returns>Claims</returns>
        protected virtual List<Claim> CreateClaims()
        {
            var claims = new List<Claim>
            {
                new(IdClaim, Id),
                new(RoleValueClaim, RoleValue.ToString())
            };

            if (Scopes != null)
            {
                claims.AddRange(Scopes.Select(scope => new Claim(ScopeClaim, scope)));
            }

            if (JsonData.Any())
            {
                var json = JsonSerializer.Serialize(JsonData, CommonJsonSerializerContext.Default.StringKeyDictionaryObject);
                claims.Add(new(JsonDataClaim, json));
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
            return new ClaimsIdentity(CreateClaims(), null, IdClaim, null);
        }
    }
}
