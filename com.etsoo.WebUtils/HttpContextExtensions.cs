﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System.Net;
using System.Net.Sockets;
using System.Security.Claims;

namespace com.etsoo.WebUtils
{
    /// <summary>
    /// HttpContext extensions
    /// HttpContext 扩展
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Get local IP address
        /// 获取本地IP地址
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns>Result</returns>
        public static IPAddress? LocalIpAddress(this HttpContext context)
        {
            return context.Connection.LocalIpAddress;
        }

        /// <summary>
        /// Get Enum item from HttpContext
        /// 从访问器中获取枚举项
        /// </summary>
        /// <typeparam name="T">Generic Enum type</typeparam>
        /// <param name="context">HttpContext</param>
        /// <param name="claimType">Claim type</param>
        /// <returns>Result</returns>
        public static T? GetEnumClaim<T>(this HttpContext context, string claimType) where T : struct, Enum
        {
            return context.User.GetEnumClaim<T>(claimType);
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
        /// Get remote IP address
        /// 获取远程IP地址
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <param name="forwarded">Includes forwarded Address or not</param>
        /// <returns>Result</returns>
        public static IPAddress? RemoteIpAddress(this HttpContext context, bool forwarded = true)
        {
            var ip = context.Connection.RemoteIpAddress;
            if (!forwarded) return ip;

            if (context.Request.Headers.TryGetValue("X-Real-IP", out var xRealIpHeader)
                && IPAddress.TryParse(xRealIpHeader.FirstOrDefault(), out var xRealIp)
                && IsIpAddressValid(xRealIp))
            {
                return xRealIp;
            }

            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var xForwardedHeader))
            {
                // Format: <client>, <proxy1>, <proxy2>,...
                var ips = xForwardedHeader.FirstOrDefault()?.Trim()
                    .Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (ips != null)
                {
                    foreach (var ipText in ips)
                    {
                        if (IPAddress.TryParse(ipText, out var fip) && IsIpAddressValid(fip))
                        {
                            return fip;
                        }
                    }
                }
            }

            return ip;
        }

        private static bool IsIpAddressValid(IPAddress ipAddress)
        {
            return ipAddress.AddressFamily is AddressFamily.InterNetwork or AddressFamily.InterNetworkV6;
        }

        /// <summary>
        /// Get user agent
        /// 获取用户代理信息
        /// </summary>
        /// <param name="context">HttpContext</param>
        /// <returns>Result</returns>
        public static string? UserAgent(this HttpContext context)
        {
            return context.Request.Headers[HeaderNames.UserAgent];
        }

        /// <summary>
        /// Write header to response
        /// 写入响应头
        /// </summary>
        /// <param name="response">HttpResponse</param>
        /// <param name="key">Header key</param>
        /// <param name="value">Header value</param>
        public static void WriteHeader(this HttpResponse response, string key, StringValues value)
        {
            response.Headers.Add(new KeyValuePair<string, StringValues>(key, value));
        }
    }
}
