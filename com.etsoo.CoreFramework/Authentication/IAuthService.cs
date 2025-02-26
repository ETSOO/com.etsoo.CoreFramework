﻿using com.etsoo.CoreFramework.User;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;

namespace com.etsoo.CoreFramework.Authentication
{
    /// <summary>
    /// Authentification service
    /// 验证服务
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Access token expiration minutes
        /// 访问令牌到期时间（分钟)
        /// </summary>
        int AccessTokenMinutes { get; }

        /// <summary>
        /// Create JWE token
        /// 创建加密令牌
        /// </summary>
        /// <param name="action">Action</param>
        /// <returns>JWE Token</returns>
        public string CreateToken(AuthAction action);

        /// <summary>
        /// Create access token
        /// 创建访问令牌
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="audience">Audience</param>
        /// <param name="liveMinutes">Live minutes</param>
        /// <returns>Token</returns>
        string CreateAccessToken(IMinUserToken user, string? audience = null, int? liveMinutes = null);

        /// <summary>
        /// Create id token
        /// 创建ID令牌
        /// </summary>
        /// <param name="claims">Claims</param>
        /// <param name="signingKey">Signing key</param>
        /// <param name="audience">Audience</param>
        /// <returns>JWS Token</returns>
        string CreateIdToken(ClaimsIdentity claims, string signingKey, string? audience = null);

        /// <summary>
        /// Create token validation parameters
        /// 创建令牌验证参数
        /// </summary>
        /// <param name="audience">Audience</param>
        /// <returns>Parameters</returns>
        TokenValidationParameters CreateValidationParameters(string? audience = null);

        /// <summary>
        /// Sign data
        /// 数据签名
        /// </summary>
        /// <param name="data">Raw UTF8 data</param>
        /// <param name="hashAlgorithm">Hash algorithm</param>
        /// <returns>RSA signature</returns>
        byte[] SignData(ReadOnlySpan<char> data, HashAlgorithmName? hashAlgorithm = null);

        /// <summary>
        /// Sign data
        /// 数据签名
        /// </summary>
        /// <param name="data">Data bytes</param>
        /// <param name="hashAlgorithm">Hash algorithm</param>
        /// <returns>RSA signature</returns>
        byte[] SignData(byte[] data, HashAlgorithmName? hashAlgorithm = null);

        /// <summary>
        /// Validate token, exception if failed
        /// 验证令牌，失败则抛出异常
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="audience">Audience</param>
        /// <returns>Claim data</returns>
        (ClaimsPrincipal? claims, SecurityToken? securityToken) ValidateToken(string token, string? audience = null);

        /// <summary>
        /// Validate id token, exception if failed
        /// 验证ID令牌，失败则抛出异常
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="issuer">Valid issuer</param>
        /// <param name="signingKey">Signing key</param>
        /// <param name="audience">Audience</param>
        /// <param name="validateLifetime">Validate the lifetime of the token</param>
        /// <returns>Claim data</returns>
        (ClaimsPrincipal? claims, SecurityToken? securityToken) ValidateIdToken(string token, string signingKey, string? issuer = null, string? audience = null, bool validateLifetime = true);

        /// <summary>
        /// Verify data
        /// 验证签名数据
        /// </summary>
        /// <param name="data">Raw UTF8 data</param>
        /// <param name="signature">Raw UTF8 signature</param>
        /// <param name="hashAlgorithm">Hash algorithm</param>
        /// <returns>Result</returns>
        bool VerifyData(ReadOnlySpan<char> data, ReadOnlySpan<char> signature, HashAlgorithmName? hashAlgorithm = null);

        /// <summary>
        /// Verify data
        /// 验证签名数据
        /// </summary>
        /// <param name="data">Date bytes</param>
        /// <param name="signature">Signature bytes</param>
        /// <param name="hashAlgorithm">Hash algorithm</param>
        /// <returns>Result</returns>
        bool VerifyData(byte[] data, byte[] signature, HashAlgorithmName? hashAlgorithm = null);
    }
}
