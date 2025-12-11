using Microsoft.AspNetCore.Http;
using System.IO.Pipelines;
using System.Net;
using System.Text;

namespace com.etsoo.WebUtils
{
    /// <summary>
    /// IHttpContextAccessor extensions
    /// IHttpContextAccessor 扩展
    /// </summary>
    public static class HttpContextAccessorExtensions
    {
        extension(IHttpContextAccessor accessor)
        {
            /// <summary>
            /// Get body content
            /// 获取请求体内容
            /// </summary>
            /// <param name="encoding">Encoding, default is UTF8</param>
            /// <returns>Result</returns>
            public async ValueTask<string?> GetBodyAsync(Encoding? encoding = null)
            {
                var context = accessor.HttpContext;

                if (context == null)
                    return null;

                return await context.GetBodyAsync(encoding);
            }

            /// <summary>
            /// Get Enum item from Accessor
            /// 从访问器中获取枚举项
            /// </summary>
            /// <typeparam name="T">Generic Enum type</typeparam>
            /// <param name="claimType">Claim type</param>
            /// <returns>Result</returns>
            public T? GetEnumClaim<T>(string claimType) where T : struct, Enum
            {
                return accessor.HttpContext?.GetEnumClaim<T>(claimType);
            }

            /// <summary>
            /// Get JSON writer
            /// 获取JSON写入器
            /// </summary>
            /// <returns>Writer</returns>
            public PipeWriter GetJsonWriter()
            {
                return accessor.HttpContext?.Response.GetJsonWriter() ?? throw new NullReferenceException("No HttpContext");
            }

            /// <summary>
            /// Get remote IP address
            /// 获取远程IP地址
            /// </summary>
            /// <param name="forwarded">Includes forwarded Address or not</param>
            /// <returns>Result</returns>
            public IPAddress? GetRemoteIpAddress(bool forwarded = true)
            {
                return accessor.HttpContext?.GetRemoteIpAddress(forwarded);
            }

            /// <summary>
            /// Set status code and write error message
            /// 设置状态码并写入错误消息
            /// </summary>
            /// <param name="statusCode">Status code</param>
            /// <param name="error">Error message</param>
            /// <returns>Task</returns>
            public ValueTask SetStatusCodeAsync(HttpStatusCode statusCode, string? error = null)
            {
                return accessor.HttpContext?.Response.SetStatusCodeAsync(statusCode, error) ?? ValueTask.CompletedTask;
            }

            /// <summary>
            /// Get cancellation token
            /// 获取取消令牌
            /// </summary>
            public CancellationToken CancellationToken => accessor.HttpContext?.RequestAborted ?? default;

            /// <summary>
            /// Get local IP address
            /// 获取本地IP地址
            /// </summary>
            public IPAddress? LocalIpAddress => accessor.HttpContext?.LocalIpAddress;

            /// <summary>
            /// Get user agent
            /// 获取用户代理信息
            /// </summary>
            public string? UserAgent => accessor.HttpContext?.UserAgent;
        }
    }
}
