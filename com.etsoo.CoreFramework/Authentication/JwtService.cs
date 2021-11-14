using com.etsoo.CoreFramework.User;
using com.etsoo.Utils.Crypto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace com.etsoo.CoreFramework.Authentication
{
    /// <summary>
    /// Jwt authentication service
    /// Jwt验证服务
    /// </summary>
    public class JwtService : IAuthService
    {
        private readonly RSACrypto crypto;
        private readonly string securityAlgorithms;

        private readonly string defaultIssuer;
        private readonly string? validIssuer;
        private readonly IEnumerable<string>? validIssuers;

        private readonly string defaultAudience;
        private readonly string? validAudience;
        private readonly IEnumerable<string>? validAudiences;

        /// <summary>
        /// Access token expiration minutes
        /// 访问令牌到期时间（分钟)
        /// </summary>
        public int AccessTokenMinutes { get; }

        /// <summary>
        /// Refresh token expiration days
        /// 刷新令牌到期时间（天）
        /// </summary>
        public int RefreshTokenDays { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="services">Dependency injection services</param>
        /// <param name="sslOnly">SSL only?</param>
        /// <param name="section">Configuration section</param>
        /// <param name="secureManager">Secure manager</param>
        public JwtService(IServiceCollection services, bool sslOnly, IConfigurationSection section, Func<string, string>? secureManager = null)
        {
            // Jwt section is required
            if (!section.Exists())
                throw new ArgumentNullException(nameof(section), "No Section");

            defaultIssuer = section.GetValue<string>("DefaultIssuer") ?? "SmartERP";
            defaultAudience = section.GetValue<string>("DefaultAudience") ?? "All";

            validIssuer = section.GetValue<string>("ValidIssuer");
            validIssuers = section.GetSection("ValidIssuers").Get<IEnumerable<string>>();
            if (string.IsNullOrEmpty(validIssuer) && validIssuers == null)
            {
                validIssuer = defaultIssuer;
            }

            validAudience = section.GetValue<string>("ValidAudience");
            validAudiences = section.GetSection("ValidAudiences").Get<IEnumerable<string>>();
            if (string.IsNullOrEmpty(validAudience) && validAudiences == null)
            {
                validAudience = defaultAudience;
            }

            // Hash algorithms
            securityAlgorithms = section.GetValue("SecurityAlgorithms", SecurityAlgorithms.RsaSha512Signature);

            // Default 30 minutes
            AccessTokenMinutes = section.GetValue("AccessTokenMinutes", 30);

            // Default 90 days
            RefreshTokenDays = section.GetValue("RefreshTokenDays", 90);

            // RSA crypto provider
            crypto = new RSACrypto(section, secureManager);

            // Adding Authentication  
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                // Is SSL only
                options.RequireHttpsMetadata = sslOnly;

                // Useful forwarding the JWT in an outgoing request
                // https://stackoverflow.com/questions/57057749/what-is-the-purpose-of-jwtbeareroptions-savetoken-property-in-asp-net-core-2-0
                options.SaveToken = false;

                // Token validation parameters
                options.TokenValidationParameters = CreateValidationParameters();
            });
        }

        private TokenValidationParameters CreateValidationParameters(bool validateLifetime = true, string? audience = null)
        {
            return new TokenValidationParameters
            {
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                RequireAudience = true,

                IssuerSigningKey = new RsaSecurityKey(crypto.RSA),

                // false to valid additional data
                ValidateLifetime = validateLifetime,

                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,

                // Specific audience
                ValidAudience = audience ?? validAudience,
                ValidAudiences = (audience == null ? validAudiences : null),

                ValidIssuer = validIssuer,
                ValidIssuers = validIssuers
            };
        }

        /// <summary>
        /// Create token
        /// 创建令牌
        /// </summary>
        /// <param name="action">Action</param>
        /// <returns>Token</returns>
        public string CreateToken(AuthAction action)
        {
            // Security key
            var securityKey = new RsaSecurityKey(crypto.RSA);

            // Enable only with private key
            if (securityKey.PrivateKeyStatus != PrivateKeyStatus.Exists)
            {
                throw new InvalidOperationException("No Private Key");
            }

            // Token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // User identity
                Subject = action.Claims,

                // TimeSpan.FromMinutes or TimeSpan.FromDays
                Expires = DateTime.UtcNow.AddTicks(action.LiveSpan.Ticks),

                // Issuer
                Issuer = validIssuer,

                // Audience
                Audience = action.Audience,

                // JWE vs JWS
                // https://stackoverflow.com/questions/33589353/what-are-the-pros-cons-of-using-jwe-or-jws
                SigningCredentials = new SigningCredentials(securityKey, securityAlgorithms)
            };

            // Create the token
            return tokenHandler.CreateEncodedJwt(tokenDescriptor);
        }

        /// <summary>
        /// Create access token
        /// 创建访问令牌
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="audience">Audience</param>
        /// <returns>Token</returns>
        public string CreateAccessToken(ICurrentUser user, string? audience = null)
        {
            return CreateToken(new AuthAction(user.CreateIdentity(), audience ?? defaultAudience, TimeSpan.FromMinutes(AccessTokenMinutes)));
        }

        private string GetRefreshTokenAudience()
        {
            return defaultIssuer + "RefreshToken";
        }

        /// <summary>
        /// Create refresh token
        /// 创建刷新令牌
        /// </summary>
        /// <param name="token">Refresh token</param>
        /// <returns>Token</returns>
        public string CreateRefreshToken(IRefreshToken token)
        {
            return CreateToken(new AuthAction(token.CreateIdentity(), GetRefreshTokenAudience(), TimeSpan.FromDays(RefreshTokenDays)));
        }

        /// <summary>
        /// Sign data
        /// 数据签名
        /// </summary>
        /// <param name="data">Raw UTF8 data</param>
        /// <param name="hashAlgorithm">Hash algorithm</param>
        /// <returns>RSA signature</returns>
        public byte[] SignData(ReadOnlySpan<char> data, HashAlgorithmName? hashAlgorithm = null)
        {
            return crypto.SignData(data, hashAlgorithm);
        }

        /// <summary>
        /// Sign data
        /// 数据签名
        /// </summary>
        /// <param name="data">Data bytes</param>
        /// <param name="hashAlgorithm">Hash algorithm</param>
        /// <returns>RSA signature</returns>
        public byte[] SignData(byte[] data, HashAlgorithmName? hashAlgorithm = null)
        {
            return crypto.SignData(data, hashAlgorithm);
        }

        /// <summary>
        /// Validate refresh token
        /// 验证刷新令牌
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="expired">Expired or not</param>
        /// <returns>Claims</returns>
        public IRefreshToken? ValidateRefreshToken(string token, out bool expired)
        {
            var handler = new JwtSecurityTokenHandler();
            var claims = handler.ValidateToken(token, CreateValidationParameters(false, GetRefreshTokenAudience()), out var validatedToken);

            expired = (validatedToken.ValidTo < DateTime.UtcNow);

            return RefreshToken.Create(claims);
        }

        /// <summary>
        /// Verify data
        /// 验证签名数据
        /// </summary>
        /// <param name="data">Raw UTF8 data</param>
        /// <param name="signature">Raw UTF8 signature</param>
        /// <param name="hashAlgorithm">Hash algorithm</param>
        /// <returns>Result</returns>
        public bool VerifyData(ReadOnlySpan<char> data, ReadOnlySpan<char> signature, HashAlgorithmName? hashAlgorithm = null)
        {
            return crypto.VerifyData(data, signature, hashAlgorithm);
        }

        /// <summary>
        /// Verify data
        /// 验证签名数据
        /// </summary>
        /// <param name="data">Date bytes</param>
        /// <param name="signature">Signature bytes</param>
        /// <param name="hashAlgorithm">Hash algorithm</param>
        /// <returns>Result</returns>
        public bool VerifyData(byte[] data, byte[] signature, HashAlgorithmName? hashAlgorithm = null)
        {
            return crypto.VerifyData(data, signature, hashAlgorithm);
        }
    }
}
