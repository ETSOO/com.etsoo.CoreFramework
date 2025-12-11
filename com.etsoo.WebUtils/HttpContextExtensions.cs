using com.etsoo.Utils.Actions;
using com.etsoo.Utils.Serialization;
using com.etsoo.Utils.String;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System.IO.Pipelines;
using System.Net;
using System.Net.Mime;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization.Metadata;

namespace com.etsoo.WebUtils
{
    /// <summary>
    /// HttpContext extensions
    /// HttpContext 扩展
    /// </summary>
    public static class HttpContextExtensions
    {
        extension(HttpContext context)
        {
            /// <summary>
            /// Get body content
            /// 获取请求体内容
            /// </summary>
            /// <param name="encoding">Encoding, default is UTF8</param>
            /// <returns>Result</returns>
            public async Task<string> GetBodyAsync(Encoding? encoding = null)
            {
                encoding ??= Encoding.UTF8;

                var sb = new StringBuilder();

                var reader = context.Request.BodyReader;

                while (true)
                {
                    var result = await reader.ReadAsync(context.RequestAborted);
                    var buffer = result.Buffer;

                    sb.Append(encoding.GetString(buffer));

                    reader.AdvanceTo(buffer.End);

                    if (result.IsCompleted)
                        break;
                }

                return sb.ToString();
            }

            /// <summary>
            /// Get Enum item from HttpContext
            /// 从访问器中获取枚举项
            /// </summary>
            /// <typeparam name="T">Generic Enum type</typeparam>
            /// <param name="claimType">Claim type</param>
            /// <returns>Result</returns>
            public T? GetEnumClaim<T>(string claimType) where T : struct, Enum
            {
                return context.User.GetEnumClaim<T>(claimType);
            }

            /// <summary>
            /// Get object from JSON body
            /// 从 JSON 请求体获取对象
            /// </summary>
            /// <typeparam name="T">Generic object type</typeparam>
            /// <returns>Result</returns>
            /// <exception cref="BadHttpRequestException"></exception>
            public async ValueTask<T> GetJsonAsync<T>()
            {
                var data = await context.Request.ReadFromJsonAsync<T>(context.RequestAborted);
                return data ?? throw new BadHttpRequestException($"Missing required JSON body of {typeof(T).FullName}");
            }

            /// <summary>
            /// Get object from JSON body with type info
            /// 通过类型信息从 JSON 请求体获取对象
            /// </summary>
            /// <typeparam name="T">Generic object type</typeparam>
            /// <param name="typeInfo">JSON type info</param>
            /// <returns>Result</returns>
            /// <exception cref="BadHttpRequestException"></exception>
            public async ValueTask<T> GetJsonAsync<T>(JsonTypeInfo<T> typeInfo)
            {
                var data = await context.Request.ReadFromJsonAsync(typeInfo, context.RequestAborted);
                return data ?? throw new BadHttpRequestException($"Missing required JSON body of {typeof(T).FullName}");
            }

            /// <summary>
            /// Get remote IP address
            /// 获取远程IP地址
            /// </summary>
            /// <param name="forwarded">Includes forwarded Address or not</param>
            /// <returns>Result</returns>
            public IPAddress? GetRemoteIpAddress(bool forwarded = true)
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

            /// <summary>
            /// Get query data
            /// 获取查询数据
            /// </summary>
            /// <param name="name">Name</param>
            /// <returns>Result</returns>
            /// <exception cref="BadHttpRequestException"></exception>
            public string GetQueryData(string name)
            {
                if (context.Request.Query.TryGetValue(name, out var values))
                {
                    return values.ToString();
                }

                throw new BadHttpRequestException($"Missing required request data {name}");
            }

            /// <summary>
            /// Get query data
            /// 获取查询数据
            /// </summary>
            /// <typeparam name="T">Generic data type</typeparam>
            /// <param name="name">Name</param>
            /// <returns>Result</returns>
            /// <exception cref="BadHttpRequestException"></exception>
            public T GetQueryData<T>(string name) where T : struct
            {
                if (context.Request.Query.TryGetValue(name, out var values))
                {
                    var v = values.ToString().Trim();
                    var data = StringUtils.TryParse<T>(v);
                    if (data.HasValue)
                    {
                        return data.Value;
                    }
                }

                throw new BadHttpRequestException($"Missing required request data {name} of {typeof(T).FullName}");
            }

            /// <summary>
            /// Get query array data
            /// 获取查询数组数据
            /// </summary>
            /// <param name="name">Name</param>
            /// <returns>Result</returns>
            /// <exception cref="BadHttpRequestException"></exception>
            public IEnumerable<string> GetQueryArray(string name)
            {
                if (context.Request.Query.TryGetValue(name, out var values))
                {
                    foreach (var value in values)
                    {
                        var v = value?.Trim();

                        if (string.IsNullOrEmpty(v)) continue;

                        yield return v;
                    }

                    yield break;
                }

                throw new BadHttpRequestException($"Missing required request array data {name}");
            }

            /// <summary>
            /// Get query array data
            /// 获取查询数组数据
            /// </summary>
            /// <typeparam name="T">Generic array item type</typeparam>
            /// <param name="name">Name</param>
            /// <returns>Result</returns>
            /// <exception cref="BadHttpRequestException"></exception>
            public IEnumerable<T> GetQueryArray<T>(string name) where T : struct
            {
                if (context.Request.Query.TryGetValue(name, out var values))
                {
                    foreach (var value in values)
                    {
                        var v = value?.Trim();

                        if (string.IsNullOrEmpty(v)) continue;

                        var item = StringUtils.TryParse<T>(v);
                        if (!item.HasValue) continue;

                        yield return item.Value;
                    }

                    yield break;
                }

                throw new BadHttpRequestException($"Missing required request array data {name} of {typeof(T).FullName}");
            }

            /// <summary>
            /// Get route data
            /// 获取路由数据
            /// </summary>
            /// <typeparam name="T">Generic data type</typeparam>
            /// <param name="name">Name</param>
            /// <returns>Result</returns>
            /// <exception cref="BadHttpRequestException"></exception>
            public T GetRouteData<T>(string name) where T : notnull
            {
                return StringUtils.TryParseObjectAll<T>(context.GetRouteValue(name)) ?? throw new BadHttpRequestException($"Missing required route data {name} of {typeof(T).FullName}");
            }

            /// <summary>
            /// Get required service
            /// 获取所需服务
            /// </summary>
            /// <typeparam name="T">Generic service type</typeparam>
            /// <returns>Result</returns>
            public T GetService<T>() where T : notnull
            {
                return context.RequestServices.GetRequiredService<T>();
            }

            /// <summary>
            /// Write object as JSON to the response body
            /// 输出对象为 JSON 到响应体
            /// </summary>
            /// <typeparam name="T">Generic data type</typeparam>
            /// <param name="data">Data</param>
            /// <param name="typeInfo">JSON type info</param>
            /// <returns>Task</returns>
            public Task WriteAsJsonAsync<T>(T data, JsonTypeInfo<T> typeInfo)
            {
                return context.Response.WriteAsJsonAsync(data, typeInfo, cancellationToken: context.RequestAborted);
            }

            /// <summary>
            /// Async write raw JSON string
            /// 异步输出原始 JSON 字符串
            /// </summary>
            /// <param name="raw">Raw JSON string</param>
            /// <returns>Task</returns>
            public ValueTask WriteRawJsonAsync(string? raw)
            {
                return context.Response.WriteRawJsonAsync(raw, context.RequestAborted);
            }

            /// <summary>
            /// Get local IP address
            /// 获取本地IP地址
            /// </summary>
            public IPAddress? LocalIpAddress => context.Connection.LocalIpAddress;

            /// <summary>
            /// Get User-Agent header
            /// 获取 User-Agent 头
            /// </summary>
            public StringValues UserAgent => context.Request.Headers.UserAgent;
        }

        extension(HttpResponse response)
        {
            /// <summary>
            /// Get JSON writer
            /// 获取JSON写入器
            /// </summary>
            /// <returns>Writer</returns>
            public PipeWriter GetJsonWriter()
            {
                response.SetJsonType();
                return response.BodyWriter;
            }

            /// <summary>
            /// Set JSON content type
            /// 设置 JSON 内容类型
            /// </summary>
            public void SetJsonType()
            {
                response.ContentType = MediaTypeNames.Application.Json;
            }

            /// <summary>
            /// Write header to response
            /// 写入响应头
            /// </summary>
            /// <param name="key">Header key</param>
            /// <param name="value">Header value</param>
            public void WriteHeader(string key, StringValues value)
            {
                response.Headers.Add(new KeyValuePair<string, StringValues>(key, value));
            }

            /// <summary>
            /// Set status code and write error message
            /// 设置状态码并写入错误消息
            /// </summary>
            /// <param name="statusCode">Status code</param>
            /// <param name="error">Error message</param>
            /// <returns>Task</returns>
            public async ValueTask SetStatusCodeAsync(HttpStatusCode statusCode, string? error = null)
            {
                response.StatusCode = (int)statusCode;
                if (!string.IsNullOrEmpty(error))
                {
                    if (error.StartsWith('{') && error.EndsWith('}'))
                    {
                        response.ContentType = "application/json";
                    }
                    else
                    {
                        response.ContentType = "text/plain";
                    }
                    await response.BodyWriter.WriteAsync(Encoding.UTF8.GetBytes(error));
                }
            }

            /// <summary>
            /// Async write raw JSON string
            /// 异步输出原始 JSON 字符串
            /// </summary>
            /// <param name="raw">Raw JSON string</param>
            /// <param name="cancellationToken">Cancellation token</param>
            /// <returns>Task</returns>
            public async ValueTask WriteRawJsonAsync(string? raw, CancellationToken cancellationToken = default)
            {
                // Content type
                response.SetJsonType();

                if (!string.IsNullOrEmpty(raw))
                    await response.WriteAsync(raw, cancellationToken);
                else
                    response.StatusCode = (int)HttpStatusCode.NoContent;
            }
        }

        extension(IActionResult result)
        {
            /// <summary>
            /// Execute action result
            /// 执行动作结果
            /// </summary>
            /// <param name="context">HTTP context</param>
            /// <returns>Task</returns>
            public Task ExecuteAsync(HttpContext context)
            {
                return context.WriteAsJsonAsync(result, CommonJsonSerializerContext.Default.IActionResult);
            }
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

        private static bool IsIpAddressValid(IPAddress ipAddress)
        {
            return ipAddress.AddressFamily is AddressFamily.InterNetwork or AddressFamily.InterNetworkV6;
        }
    }
}
