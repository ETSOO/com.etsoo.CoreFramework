using com.etsoo.CoreFramework.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
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

        readonly string issuer;
        readonly string audience;

        /// <summary>
        /// Access token expiration minutes
        /// 访问令牌到期时间（分钟)
        /// </summary>
        public int AccessTokenMinutes { get; }

        /// <summary>
        /// Refresh token expiration hours
        /// 刷新令牌到期时间（小时）
        /// </summary>
        public int RefreshTokenHours { get; }

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
            securityAlgorithms = section.GetValue("SecurityAlgorithms", SecurityAlgorithms.HmacSha512);
            issuer = section.GetValue("Issuer", "SmartERP");
            audience = section.GetValue("Audience", "all");

            AccessTokenMinutes = section.GetValue("AccessTokenMinutes", 15);
            RefreshTokenHours = section.GetValue("RefreshTokenHours", 360); // 15 days x 24 = 360

            // Adding Authentication  
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                // Is SSL only
                options.RequireHttpsMetadata = sslOnly;

                // Useful forwarding the JWT in an outgoing request
                // https://stackoverflow.com/questions/57057749/what-is-the-purpose-of-jwtbeareroptions-savetoken-property-in-asp-net-core-2-0
                options.SaveToken = true;

                // Token validation parameters
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    RequireSignedTokens = true,
                    RequireExpirationTime = true,
                    RequireAudience = true,

                    IssuerSigningKey = new SymmetricSecurityKey(securityKeyBytes),

                    ValidateLifetime = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,

                    ValidAudience = section.GetValue<string>("ValidAudience"),
                    ValidAudiences = section.GetSection("ValidAudiences").Get<IEnumerable<string>>(),
                    ValidIssuer = section.GetValue<string>("ValidIssuer"),
                    ValidIssuers = section.GetSection("ValidIssuers").Get<IEnumerable<string>>()
                };
            });
        }

        /// <summary>
        /// Create token
        /// 创建令牌
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="liveSpan">Live time span</param>
        /// <returns>Token</returns>
        public string CreateToken(ICurrentUser user, TimeSpan liveSpan)
        {
            // Token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // User identity
                Subject = user.CreateIdentity(),

                // TimeSpan.FromMinutes or TimeSpan.FromDays
                Expires = DateTime.UtcNow.AddTicks(liveSpan.Ticks),

                // Issuer
                Issuer = issuer,

                // Audience
                Audience = audience,

                // JWE vs JWS
                // https://stackoverflow.com/questions/33589353/what-are-the-pros-cons-of-using-jwe-or-jws
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(securityKeyBytes), securityAlgorithms)
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
            return CreateToken(user, TimeSpan.FromMinutes(AccessTokenMinutes));
        }

        /// <summary>
        /// Create refresh token
        /// 创建刷新令牌
        /// </summary>
        /// <returns>Token</returns>
        public string CreateRefreshToken()
        {
            Span<byte> randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
