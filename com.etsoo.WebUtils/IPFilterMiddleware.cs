using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace com.etsoo.WebUtils
{
    /// <summary>
    /// IP filter middleware configuration
    /// IP过滤器中间件配置
    /// </summary>
    internal class IPFilterMiddlewareConfig
    {
        /// <summary>
        /// Whitelist
        /// 白名单
        /// </summary>
        public IEnumerable<string>? Whitelist { get; init; }

        /// <summary>
        /// Blacklist
        /// 黑名单
        /// </summary>
        public IEnumerable<string>? Blacklist { get; init; }
    }

    /// <summary>
    /// IP filter middleware
    /// IP过滤器中间件
    /// </summary>
    public class IPFilterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IEnumerable<IPAddress>? _whitelist;
        private readonly IEnumerable<IPAddress>? _blacklist;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="next">Next</param>
        /// <param name="configuration">Configuration</param>
        /// <param name="configPath">Configuration section path</param>
        public IPFilterMiddleware(RequestDelegate next, IConfiguration configuration, string configPath)
        {
            _next = next;

            var section = configuration.GetSection(configPath).Get<IPFilterMiddlewareConfig>();
            if (section != null)
            {
                _whitelist = section.Whitelist?.Select(IPAddress.Parse);
                _blacklist = section.Blacklist?.Select(IPAddress.Parse);
            }
        }

        /// <summary>
        /// Async Invoke
        /// 异步调用
        /// </summary>
        /// <param name="context">Context</param>
        /// <returns>Task</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var remoteIp = context.Connection.RemoteIpAddress;
            if (remoteIp.IsIPv4MappedToIPv6) remoteIp = remoteIp.MapToIPv4();

            if (
                (_whitelist != null && !(_whitelist.Any(ip => ip.Equals(remoteIp))))
                ||
                (_blacklist != null && _blacklist.Any(ip => ip.Equals(remoteIp)))
            )
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return;
            }

            await _next.Invoke(context);
        }
    }

    /// <summary>
    /// IP filter middleware extensions
    /// IP过滤器中间件扩展
    /// </summary>
    public static class IPFilterMiddlewareExtensions
    {
        public static IApplicationBuilder UseIPFilter(
            this IApplicationBuilder builder, string configPath = "Etsoo:IPFilter")
        {
            return builder.UseMiddleware<IPFilterMiddleware>(configPath);
        }
    }
}
