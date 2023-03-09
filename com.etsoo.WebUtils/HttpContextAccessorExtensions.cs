using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
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
            return accessor.HttpContext.RequestAborted;
        }

        /// <summary>
        /// Get local IP address
        /// 获取本地IP地址
        /// </summary>
        /// <param name="accessor">Accessor</param>
        /// <returns>Result</returns>
        public static IPAddress LocalIpAddress(this IHttpContextAccessor accessor)
        {
            return accessor.HttpContext.Connection.LocalIpAddress;
        }

        /// <summary>
        /// Get remote IP address
        /// 获取远程IP地址
        /// </summary>
        /// <param name="accessor">Accessor</param>
        /// <returns>Result</returns>
        public static IPAddress RemoteIpAddress(this IHttpContextAccessor accessor)
        {
            return accessor.HttpContext.Connection.RemoteIpAddress;
        }

        /// <summary>
        /// Get user agent
        /// 获取用户代理信息
        /// </summary>
        /// <param name="accessor">Accessor</param>
        /// <returns>Result</returns>
        public static string? UserAgent(this IHttpContextAccessor accessor)
        {
            return accessor.HttpContext.Request.Headers[HeaderNames.UserAgent];
        }
    }
}
