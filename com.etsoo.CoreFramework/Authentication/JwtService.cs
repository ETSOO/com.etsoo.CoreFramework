using com.etsoo.CoreFramework.User;
using com.etsoo.Utils.Crypto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace com.etsoo.CoreFramework.Authentication
{
    /// <summary>
    /// Jwt authentication service
    /// https://andrewlock.net/a-look-behind-the-jwt-bearer-authentication-middleware-in-asp-net-core/
    /// https://stackoverflow.com/questions/49694383/use-multiple-jwt-bearer-authentication or IssuerSigningKeys
    /// Jwt验证服务
    /// </summary>
    public class JwtService : IAuthService
    {
        private const string DefaultIssuer = "SmartERP";

        private readonly RSACrypto crypto;
        private readonly string securityAlgorithms;

        private readonly string defaultIssuer;
        private readonly string validIssuer;
        private readonly IEnumerable<string>? validIssuers;

        private readonly string defaultAudience;
        private readonly string? validAudience;
        private readonly IEnumerable<string>? validAudiences;

        private readonly string encryptionKey;

        /// <summary>
        /// Access token expiration minutes
        /// 访问令牌到期时间（分钟)
        /// </summary>
        public int AccessTokenMinutes { get; }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="services">Dependency injection services</param>
        /// <param name="settings">Settings</param>
        /// <param name="events">Events handler</param>
        public JwtService(IServiceCollection services,
            JwtSettings? settings,
            JwtBearerEvents? events = null)
        {
            // Jwt settings are required
            ArgumentNullException.ThrowIfNull(settings, nameof(settings));

            defaultIssuer = settings.DefaultIssuer ?? DefaultIssuer;
            defaultAudience = settings.DefaultAudience ?? "ALL";

            validIssuers = settings.ValidIssuers;
            if (string.IsNullOrEmpty(settings.ValidIssuer))
            {
                validIssuer = defaultIssuer;
            }
            else
            {
                validIssuer = settings.ValidIssuer;
            }

            validAudience = settings.ValidAudience;
            validAudiences = settings.ValidAudiences;
            if (string.IsNullOrEmpty(validAudience) && validAudiences == null)
            {
                validAudience = defaultAudience;
            }

            var tokenUrls = settings.TokenUrls;

            // Hash algorithms
            securityAlgorithms = settings.SecurityAlgorithms ?? SecurityAlgorithms.RsaSha512Signature;

            // Default 30 minutes
            AccessTokenMinutes = settings.AccessTokenMinutes.GetValueOrDefault(30);

            // https://stackoverflow.com/questions/53487247/encrypting-jwt-security-token-supported-algorithms
            // AES256, 256 / 8 = 32 bytes
            encryptionKey = settings.EncryptionKey;

            // RSA crypto provider
            crypto = new RSACrypto(settings.PublicKey, settings.PrivateKey);

            // Sending the access token in the query string is required due to
            // a limitation in Browser APIs. We restrict it to only calls to the
            // SignalR hub in this code.
            // See https://docs.microsoft.com/aspnet/core/signalr/security#access-token-logging
            // for more information about security considerations when using
            // the query string to transmit the access token.
            if (tokenUrls?.Any() is true)
            {
                void messageReceived(MessageReceivedContext context)
                {
                    var path = context.HttpContext.Request.Path;
                    if (tokenUrls.Any(url => path.StartsWithSegments(url)))
                    {
                        var accessToken = context.Request.Query["access_token"];

                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                    }
                }

                if (events == null)
                {
                    events = new JwtBearerEvents
                    {
                        OnMessageReceived = (context) =>
                        {
                            messageReceived(context);
                            return Task.CompletedTask;
                        }
                    };
                }
                else
                {
                    var OnMessageReceived = events.OnMessageReceived;
                    events.OnMessageReceived = (context) =>
                    {
                        messageReceived(context);
                        return OnMessageReceived(context);
                    };
                }
            }

            // Adding Authentication  
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                // Events handlers
                if (events != null)
                    options.Events = events;

                // Useful forwarding the JWT in an outgoing request
                // https://stackoverflow.com/questions/57057749/what-is-the-purpose-of-jwtbeareroptions-savetoken-property-in-asp-net-core-2-0
                options.SaveToken = false;

                // Token validation parameters
                options.TokenValidationParameters = CreateValidationParameters();
            });
        }

        private TokenValidationParameters CreateValidationParametersBase(string? audience = null)
        {
            return new TokenValidationParameters
            {
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                RequireAudience = true,

                // false to valid additional data
                ValidateLifetime = true,

                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,

                // Specific audience
                ValidAudience = audience ?? validAudience,
                ValidAudiences = audience == null ? validAudiences : null,

                ValidIssuer = validIssuer,
                ValidIssuers = validIssuers
            };
        }

        private TokenValidationParameters CreateValidationParameters(string? audience = null)
        {
            var parameters = CreateValidationParametersBase(audience);
            parameters.IssuerSigningKey = GetIssuerSigningKey();
            parameters.TokenDecryptionKey = GetTokenDecryptionKey();
            return parameters;
        }

        private TokenValidationParameters CreateIdTokenValidationParameters(string signingKey, string? issuer = null, string? audience = null)
        {
            // Security key
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));

            var parameters = CreateValidationParametersBase(audience);
            parameters.IssuerSigningKey = securityKey;
            parameters.ValidIssuer = issuer;
            parameters.ValidateIssuer = !string.IsNullOrEmpty(issuer);

            return parameters;
        }

        /// <summary>
        /// Create JWE token
        /// 创建加密令牌
        /// </summary>
        /// <param name="action">Action</param>
        /// <returns>JWE Token</returns>
        public string CreateToken(AuthAction action)
        {
            // Token validation parameters
            var validataionParameters = new TokenValidationParameters
            {
                ValidIssuer = validIssuer,
                ValidIssuers = validIssuers,
                ValidAudience = action.Audience
            };

            // Security key
            var securityKey = GetIssuerSigningKey();
            var encryptionKey = GetTokenDecryptionKey();

            // Enable only with private key
            if (securityKey is RsaSecurityKey sk && sk.PrivateKeyStatus != PrivateKeyStatus.Exists)
            {
                throw new InvalidOperationException("No Issuer Signing Security Key");
            }

            // Token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // User identity
                Subject = action.Claims,

                // Expires
                Expires = DateTime.UtcNow.AddTicks(action.LiveSpan.Ticks),

                // Issuer
                Issuer = validataionParameters.ValidIssuer,

                // Audience
                Audience = validataionParameters.ValidAudience,

                // JWE vs JWS
                // https://stackoverflow.com/questions/33589353/what-are-the-pros-cons-of-using-jwe-or-jws
                SigningCredentials = new SigningCredentials(securityKey, securityAlgorithms),
                EncryptingCredentials = new EncryptingCredentials(encryptionKey, SecurityAlgorithms.Aes256KeyWrap, SecurityAlgorithms.Aes256CbcHmacSha512)
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
        /// <param name="liveMinutes">Live minutes</param>
        /// <returns>JWE Token</returns>
        public string CreateAccessToken(IMinUserToken user, string? audience = null, int? liveMinutes = null)
        {
            return CreateToken(new AuthAction(user.CreateIdentity(), audience ?? defaultAudience, TimeSpan.FromMinutes(liveMinutes.GetValueOrDefault(AccessTokenMinutes))));
        }

        /// <summary>
        /// Create id token
        /// 创建ID令牌
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <param name="signingKey">Signing key</param>
        /// <param name="audience">Audience</param>
        /// <returns>JWS Token</returns>
        public string CreateIdToken(ClaimsIdentity claims, string signingKey, string? audience = null)
        {
            // Token validation parameters
            var validataionParameters = new TokenValidationParameters
            {
                ValidIssuer = validIssuer,
                ValidIssuers = validIssuers,
                ValidAudience = audience
            };

            // SHA256 requires 256 bits key
            if (signingKey.Length < 32)
            {
                throw new ArgumentException("Signing key length must be at least 32 characters");
            }

            // Security key
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));

            // Token handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // Token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // User identity
                Subject = claims,

                // Issuer
                Issuer = validataionParameters.ValidIssuer,

                // Audience
                Audience = validataionParameters.ValidAudience,

                // Expires in 5 minutes
                Expires = DateTime.UtcNow.AddMinutes(5),

                // Signing credentials
                SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
            };

            // Create the token
            return tokenHandler.CreateEncodedJwt(tokenDescriptor);
        }

        /// <summary>
        /// Get issuer signing key
        /// 获取签发密钥
        /// </summary>
        /// <returns>Security key</returns>
        protected virtual SecurityKey GetIssuerSigningKey()
        {
            return new RsaSecurityKey(crypto.RSA);
        }

        /// <summary>
        /// Get token decryption key
        /// 获取令牌解密密钥
        /// </summary>
        /// <returns>Security key</returns>
        protected virtual SecurityKey GetTokenDecryptionKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encryptionKey));
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
        /// Validate token, exception if failed
        /// 验证令牌，失败则抛出异常
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="audience">Audience</param>
        /// <returns>Claim data</returns>
        public (ClaimsPrincipal? claims, SecurityToken? securityToken) ValidateToken(string token, string? audience = null)
        {
            var handler = new JwtSecurityTokenHandler();
            var claims = handler.ValidateToken(token, CreateValidationParameters(audience), out var validatedToken);

            // var securityToken = validatedToken as JwtSecurityToken;
            // var keyId = validatedToken is JwtSecurityToken jk ? jk.Header.Kid : (validatedToken.SecurityKey?.KeyId);

            return (claims, validatedToken);
        }

        /// <summary>
        /// Validate id token, exception if failed
        /// 验证ID令牌，失败则抛出异常
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="issuer">Valid issuer</param>
        /// <param name="signingKey">Signing key</param>
        /// <param name="audience">Audience</param>
        /// <returns>Claim data</returns>
        public (ClaimsPrincipal? claims, SecurityToken? securityToken) ValidateIdToken(string token, string signingKey, string? issuer = null, string? audience = null)
        {
            var handler = new JwtSecurityTokenHandler();
            var claims = handler.ValidateToken(token, CreateIdTokenValidationParameters(signingKey, issuer, audience), out var validatedToken);
            return (claims, validatedToken);
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
