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

        private readonly TokenDecryptionKeyResolver tokenDecryptionKeyResolver;

        private readonly RSACrypto crypto;
        private readonly string securityAlgorithms;
        private readonly IssuerSigningKeyResolver issuerSigningKeyResolver;

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
        /// <param name="settings">Settings</param>
        /// <param name="issuerSigningKeyResolver">Issuer signing key resolver</param>
        /// <param name="tokenDecryptionKeyResolver">Token decryption key resolver</param>
        /// <param name="events">Events handler</param>
        public JwtService(IServiceCollection services,
            JwtSettings? settings,
            IssuerSigningKeyResolver? issuerSigningKeyResolver = null,
            TokenDecryptionKeyResolver? tokenDecryptionKeyResolver = null,
            JwtBearerEvents? events = null)
        {
            // Jwt settings are required
            ArgumentNullException.ThrowIfNull(settings, nameof(settings));

            defaultIssuer = settings.DefaultIssuer ?? DefaultIssuer;
            defaultAudience = settings.DefaultAudience ?? "All";

            validIssuer = settings.ValidIssuer;
            validIssuers = settings.ValidIssuers;
            if (string.IsNullOrEmpty(validIssuer))
            {
                validIssuer = defaultIssuer;
            }

            validAudience = settings.ValidAudience;
            validAudiences = settings.ValidAudiences;
            if (string.IsNullOrEmpty(validAudience) && validAudiences == null)
            {
                validAudience = defaultAudience;
            }

            var tokenUrls = settings.TokenUrls;

            // Whether validate audience
            var validateAudience = settings.ValidateAudience;

            // Hash algorithms
            securityAlgorithms = settings.SecurityAlgorithms ?? SecurityAlgorithms.RsaSha512Signature;

            // Default 30 minutes
            AccessTokenMinutes = settings.AccessTokenMinutes.GetValueOrDefault(30);

            // Default 90 days
            RefreshTokenDays = settings.RefreshTokenDays.GetValueOrDefault(90);

            // https://stackoverflow.com/questions/53487247/encrypting-jwt-security-token-supported-algorithms
            // AES256, 256 / 8 = 32 bytes
            var encryptionKey = settings.EncryptionKey;

            // RSA crypto provider
            crypto = new RSACrypto(settings.PublicKey, settings.PrivateKey);

            // Default signing key resolver
            this.issuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
            {
                if (issuerSigningKeyResolver == null)
                {
                    return new List<RsaSecurityKey> { new(crypto.RSA) { KeyId = kid } };
                }

                var keys = issuerSigningKeyResolver(token, securityToken, kid, validationParameters);
                if (!keys.Any()) keys = keys.Append(new RsaSecurityKey(crypto.RSA) { KeyId = kid });

                return keys;
            };

            this.tokenDecryptionKeyResolver = (token, securityToken, kid, validationParameters) =>
            {
                if (tokenDecryptionKeyResolver == null)
                {
                    return new List<SymmetricSecurityKey> { new(Encoding.UTF8.GetBytes(encryptionKey)) { KeyId = kid } };
                }

                var keys = tokenDecryptionKeyResolver(token, securityToken, kid, validationParameters);
                if (!keys.Any()) keys = keys.Append(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encryptionKey)) { KeyId = kid });

                return keys;
            };

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
                if (validateAudience != null)
                    options.TokenValidationParameters.ValidateAudience = validateAudience.Value;
            });
        }

        private TokenValidationParameters CreateValidationParameters(bool validateLifetime = true, string? audience = null)
        {
            return new TokenValidationParameters
            {
                RequireSignedTokens = true,
                RequireExpirationTime = true,
                RequireAudience = true,

                //IssuerSigningKey = new RsaSecurityKey(crypto.RSA),
                IssuerSigningKeyResolver = issuerSigningKeyResolver,

                // Token decryption
                // TokenDecryptionKey = encryptionKey,
                TokenDecryptionKeyResolver = tokenDecryptionKeyResolver,

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
            // Token validation parameters
            var validataionParameters = new TokenValidationParameters
            {
                ValidIssuer = validIssuer,
                ValidIssuers = validIssuers,
                ValidAudience = action.Audience
            };

            // Security key
            var keys = issuerSigningKeyResolver(string.Empty, null, action.KeyId, validataionParameters);
            var securityKey = string.IsNullOrEmpty(action.KeyId) || action.KeyId == DefaultIssuer ? keys.FirstOrDefault() : keys.FirstOrDefault(item => !string.IsNullOrEmpty(item.KeyId) && item.KeyId.Equals(action.KeyId));

            var encryptionKeys = tokenDecryptionKeyResolver(string.Empty, null, action.KeyId, validataionParameters);
            var encryptionKey = string.IsNullOrEmpty(action.KeyId) || action.KeyId == DefaultIssuer ? encryptionKeys.FirstOrDefault() : encryptionKeys.FirstOrDefault(item => !string.IsNullOrEmpty(item.KeyId) && item.KeyId.Equals(action.KeyId));

            // Enable only with private key
            if (securityKey == null || encryptionKey == null || (securityKey is RsaSecurityKey sk && sk.PrivateKeyStatus != PrivateKeyStatus.Exists))
            {
                throw new InvalidOperationException("No Security Key");
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
        /// <param name="keyId">Key id</param>
        /// <returns>Token</returns>
        public string CreateAccessToken(IServiceUser user, string? audience = null, string? keyId = null)
        {
            user.Service = keyId;
            return CreateToken(new AuthAction(user.CreateIdentity(), audience ?? defaultAudience, TimeSpan.FromMinutes(AccessTokenMinutes), keyId));
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
        /// <param name="validMinutes">Valid minutes</param>
        /// <returns>Token</returns>
        public string CreateRefreshToken(IRefreshToken token, int? validMinutes = null)
        {
            var ts = validMinutes == null ? TimeSpan.FromDays(RefreshTokenDays) : TimeSpan.FromMinutes(validMinutes.Value);
            return CreateToken(new AuthAction(token.CreateIdentity(), GetRefreshTokenAudience(), ts, token.Sid));
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
        /// Validate token
        /// 验证令牌
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="audience">Audience</param>
        /// <returns>Claim data</returns>
        public (ClaimsPrincipal? claims, bool expired, string? kid, SecurityToken? securityToken) ValidateToken(string token, string? audience = null)
        {
            var handler = new JwtSecurityTokenHandler();
            var claims = handler.ValidateToken(token, CreateValidationParameters(false, audience ?? GetRefreshTokenAudience()), out var validatedToken);

            var securityToken = validatedToken as JwtSecurityToken;
            var expired = (validatedToken.ValidTo < DateTime.UtcNow);
            var kid = validatedToken is JwtSecurityToken jk ? jk.Header.Kid : (validatedToken.SecurityKey?.KeyId);

            return (claims, expired, kid, securityToken);
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
