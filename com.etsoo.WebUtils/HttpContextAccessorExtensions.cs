using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Net;
using System.Security.Claims;

namespace com.etsoo.WebUtils
{
    /// <summary>
    /// IHttpContextAccessor extensions
    /// IHttpContextAccessor 扩展
    /// </summary>
    public static class HttpContextAccessorExtensions
    {
        /// <summary>
        /// Get cancellation token
        /// 获取取消令牌
        /// </summary>
        /// <param name="accessor">Accessor</param>
        /// <returns>Result</returns>
        public static CancellationToken CancellationToken(this IHttpContextAccessor accessor)
        {
            return accessor.HttpContext?.RequestAborted ?? default;
        }

        /// <summary>
        /// Get Enum item from Accessor
        /// 从访问器中获取枚举项
        /// </summary>
        /// <typeparam name="T">Generic Enum type</typeparam>
        /// <param name="accessor">Accessor</param>
        /// <param name="claimType">Claim type</param>
        /// <returns>Result</returns>
        public static T? GetEnumClaim<T>(this IHttpContextAccessor accessor, string claimType) where T : struct, Enum
        {
            return accessor.HttpContext?.User.GetEnumClaim<T>(claimType);
        }

        /// <summary>
        /// Get Enum item from claim
        /// 从声明中获取枚举项
        /// </summary>
        /// <typeparam name="T">Generic Enum type</typeparam>
        /// <param name="principal">Claims principal</param>
        /// <param name="claimType">Claim type</param>
        /// <returns>Result</returns>
        public static T? GetEnumClaim<T>(this ClaimsPrincipal principal, string claimType) where T : struct, Enum
        {
            var claimValue = principal.FindFirst(claimType)?.Value;

            if (!string.IsNullOrEmpty(claimValue) && Enum.TryParse<T>(claimValue, out var p))
            {
                return p;
            }

            return default;
        }

        /// <summary>
        /// Get local IP address
        /// 获取本地IP地址
        /// </summary>
        /// <param name="accessor">Accessor</param>
        /// <returns>Result</returns>
        public static IPAddress? LocalIpAddress(this IHttpContextAccessor accessor)
        {
            return accessor.HttpContext?.Connection.LocalIpAddress;
        }

        /// <summary>
        /// Get remote IP address
        /// 获取远程IP地址
        /// </summary>
        /// <param name="accessor">Accessor</param>
        /// <returns>Result</returns>
        public static IPAddress? RemoteIpAddress(this IHttpContextAccessor accessor)
        {
            return accessor.HttpContext?.Connection.RemoteIpAddress;
        }

        /// <summary>
        /// Get user agent
        /// 获取用户代理信息
        /// </summary>
        /// <param name="accessor">Accessor</param>
        /// <returns>Result</returns>
        public static string? UserAgent(this IHttpContextAccessor accessor)
        {
            return accessor.HttpContext?.Request.Headers[HeaderNames.UserAgent];
        }
    }
}
