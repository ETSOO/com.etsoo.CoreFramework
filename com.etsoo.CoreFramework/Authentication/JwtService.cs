using com.etsoo.CoreFramework.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace com.etsoo.CoreFramework.Authentication
{
    /// <summary>
    /// Jwt authentication service
    /// Jwt验证服务
    /// </summary>
    public class JwtService : IAuthService
    {
        readonly string securityAlgorithms;
        readonly byte[] securityKeyBytes;
        readonly byte[] securityKeyBytesFK;

        readonly string issuer;
        readonly string audience;

        private const string refreshTokenAudience = "RefreshToken";
        readonly TokenValidationParameters refreshTokenParameters;

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
        /// <param name="securityKey">Security key</param>
        public JwtService(IServiceCollection services, bool sslOnly, IConfigurationSection section, string securityKey)
            : this(services, sslOnly, section, Encoding.UTF8.GetBytes(securityKey))
        {
        }

        private byte[] MakeRefreshTokenKey()
        {
            // Length bytes
            var sLen = securityKeyBytes.Length;
            var intBytes = BitConverter.GetBytes(sLen);
            var intLen = intBytes.Length;

            // New bytes
            var bLen = intLen + sLen;
            var bytes = new byte[bLen];

            // Copy
            intBytes.CopyTo(bytes, 0);
            securityKeyBytes.CopyTo(bytes, intLen);

            // Change 8, 16, 32, 64, 128 position bytes
            var posItems = new byte[] { 8, 16, 32, 64, 128 };
            foreach(var pos in posItems)
            {
                if (pos >= bLen)
                    break;

                bytes[pos] &= 2;
            }

            return bytes;
        }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="services">Dependency injection services</param>
        /// <param name="sslOnly">SSL only?</param>
        /// <param name="section">Configuration section</param>
        /// <param name="keyBytes">Security key bytes</param>
        public JwtService(IServiceCollection services, bool sslOnly, IConfigurationSection section, byte[] keyBytes)
        {
            // Jwt section is required
            if (!section.Exists())
                throw new ArgumentNullException(nameof(section));

            securityKeyBytes = keyBytes;

            // Calculate a different security key bytes
            securityKeyBytesFK = MakeRefreshTokenKey();

            securityAlgorithms = section.GetValue("SecurityAlgorithms", SecurityAlgorithms.HmacSha512);

            issuer = section.GetValue("Issuer", "SmartERP");
            audience = section.GetValue("Audience", "access");

            var validIssuer = section.GetValue<string>("ValidIssuer");
            var validIssuers = section.GetSection("ValidIssuers").Get<IEnumerable<string>>();
            if (string.IsNullOrEmpty(validIssuer) && validIssuers == null)
            {
                validIssuer = issuer;
            }

            var validAudience = section.GetValue<string>("ValidAudience");
            var validAudiences = section.GetSection("ValidAudiences").Get<IEnumerable<string>>();
            if (string.IsNullOrEmpty(validAudience) && validAudiences == null)
            {
                validAudience = audience;
            }

            AccessTokenMinutes = section.GetValue("AccessTokenMinutes", 15);
            RefreshTokenDays = section.GetValue("RefreshTokenDays", 90);

            var parameters = new TokenValidationParameters
            {
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                RequireAudience = true,

                IssuerSigningKey = new SymmetricSecurityKey(securityKeyBytes),

                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,

                ValidAudience = validAudience,
                ValidAudiences = validAudiences,
                ValidIssuer = validIssuer,
                ValidIssuers = validIssuers
            };

            refreshTokenParameters = new TokenValidationParameters
            {
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                RequireAudience = true,

                IssuerSigningKey = new SymmetricSecurityKey(securityKeyBytesFK),

                ValidateLifetime = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,

                ValidAudience = refreshTokenAudience,
                ValidIssuer = validIssuer,
                ValidIssuers = validIssuers
            };

            // Adding Authentication  
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                // Is SSL only
                options.RequireHttpsMetadata = sslOnly;

                // Useful forwarding the JWT in an outgoing request
                // https://stackoverflow.com/questions/57057749/what-is-the-purpose-of-jwtbeareroptions-savetoken-property-in-asp-net-core-2-0
                options.SaveToken = true;

                // Token validation parameters
                options.TokenValidationParameters = parameters;
            });
        }

        /// <summary>
        /// Create token
        /// 创建令牌
        /// </summary>
        /// <param name="action">Action</param>
        /// <returns>Token</returns>
        public string CreateToken(AuthAction action)
        {
            // Token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // User identity
                Subject = action.User.CreateIdentity(),

                // TimeSpan.FromMinutes or TimeSpan.FromDays
                Expires = DateTime.UtcNow.AddTicks(action.LiveSpan.Ticks),

                // Issuer
                Issuer = issuer,

                // Audience
                Audience = action.Audience,

                // JWE vs JWS
                // https://stackoverflow.com/questions/33589353/what-are-the-pros-cons-of-using-jwe-or-jws
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(action.SecurityKeyBytes), securityAlgorithms)
            };

            // Create the token
            return tokenHandler.CreateEncodedJwt(tokenDescriptor);
        }

        /// <summary>
        /// Create access token
        /// 创建访问令牌
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Token</returns>
        public string CreateAccessToken(ICurrentUser user)
        {
            return CreateToken(new AuthAction(user, audience, TimeSpan.FromMinutes(AccessTokenMinutes), securityKeyBytes));
        }

        /// <summary>
        /// Create refresh token
        /// 创建刷新令牌
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Token</returns>
        public string CreateRefreshToken(ICurrentUser user)
        {
            /*
            /* This method just like a random password
            Span<byte> randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
            */

            return CreateToken(new AuthAction(user, refreshTokenAudience, TimeSpan.FromDays(RefreshTokenDays), securityKeyBytesFK));
        }

        /// <summary>
        /// Validate refresh token
        /// 验证刷新令牌
        /// </summary>
        /// <param name="token">Token</param>
        /// <returns>Claims</returns>
        public ClaimsPrincipal? ValidateRefreshToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                return handler.ValidateToken(token, refreshTokenParameters, out _);
            }
            catch
            {
                return null;
            }
        }
    }
}
