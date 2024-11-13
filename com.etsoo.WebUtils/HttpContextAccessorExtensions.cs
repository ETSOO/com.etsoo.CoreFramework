using Microsoft.AspNetCore.Http;
using System.IO.Pipelines;
using System.Net;

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
            return accessor.HttpContext?.GetEnumClaim<T>(claimType);
        }

        /// <summary>
        /// Get JSON writer
        /// 获取JSON写入器
        /// </summary>
        /// <param name="accessor">HttpContextAccessor</param>
        /// <returns>Writer</returns>
        /// <exception cref="NullReferenceException">Null Reference Exception</exception>
        public static PipeWriter GetJsonWriter(this IHttpContextAccessor accessor)
        {
            var response = accessor.HttpContext?.Response ?? throw new NullReferenceException("No HttpContext");
            return response.GetJsonWriter();
        }

        /// <summary>
        /// Get local IP address
        /// 获取本地IP地址
        /// </summary>
        /// <param name="accessor">Accessor</param>
        /// <returns>Result</returns>
        public static IPAddress? LocalIpAddress(this IHttpContextAccessor accessor)
        {
            return accessor.HttpContext?.LocalIpAddress();
        }

        /// <summary>
        /// Get remote IP address
        /// 获取远程IP地址
        /// </summary>
        /// <param name="accessor">Accessor</param>
        /// <param name="forwarded">Includes forwarded Address or not</param>
        /// <returns>Result</returns>
        public static IPAddress? RemoteIpAddress(this IHttpContextAccessor accessor, bool forwarded = true)
        {
            return accessor.HttpContext?.RemoteIpAddress(forwarded);
        }

        /// <summary>
        /// Get user agent
        /// 获取用户代理信息
        /// </summary>
        /// <param name="accessor">Accessor</param>
        /// <returns>Result</returns>
        public static string? UserAgent(this IHttpContextAccessor accessor)
        {
            return accessor.HttpContext?.UserAgent();
        }
    }
}
