using com.etsoo.ApiModel.Auth;
using com.etsoo.CoreFramework.Services;
using com.etsoo.UserAgentParser;
using com.etsoo.Utils.Actions;
using com.etsoo.WebUtils;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace com.etsoo.Web
{
    /// <summary>
    /// Setup extensions
    /// 设置扩展
    /// </summary>
    public static class SetupExtensions
    {
        /// <summary>
        /// Get OAuth2 user info from client
        /// 从客户端获取OAuth2用户信息
        /// </summary>
        /// <param name="service">Service</param>
        /// <param name="client">OAuth2 client</param>
        /// <param name="context">HTPP context</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result & User info & login validation data</returns>
        public static async Task<(IActionResult result, AuthUserInfo? userInfo, AuthLoginValidateData? loginData)> GetUserInfoAsync(this IServiceBase service, IAuthClient client, HttpContext context, CancellationToken cancellationToken)
        {
            (string DeviceCore, UAParser Parser)? parser = null;
            string? region = null;
            string? deviceId = null;

            var (result, userInfo, state) = await client.GetUserInfoAsync(context.Request, (s) =>
            {
                // We put the region code like 'CN' at the beginning of the device id
                region = s[..2];

                // The device id is the rest of the string
                deviceId = s[2..];

                return service.CheckDevice(context.UserAgent(), deviceId.Replace(" ", "+"), out _, out parser);
            }, null, cancellationToken);

            AuthLoginValidateData? data = null;
            if (parser != null && region != null && deviceId != null)
            {
                data = new AuthLoginValidateData
                {
                    DeviceId = deviceId,
                    Region = region,
                    Parser = parser.Value.Parser
                };
            }

            return (result, userInfo, data);
        }

        /// <summary>
        /// Setup Cors policy
        /// </summary>
        /// <param name="builder">Builder</param>
        /// <param name="options">Options</param>
        public static void Setup(this CorsPolicyBuilder builder, CorsPolicySetupOptions options)
        {
            // .WithOrigins(Cors)
            // .SetIsOriginAllowedToAllowWildcardSubdomains()
            builder.SetIsOriginAllowed(options.Check)

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Access-Control-Allow-Credentials
                // https://stackoverflow.com/questions/24687313/what-exactly-does-the-access-control-allow-credentials-header-do
                // JWT is not a cookie solution, disable it without allow credential
                .AllowCredentials()
                //.DisallowCredentials()

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers
                // Without it will popup error: Request header field content-type is not allowed by Access-Control-Allow-Headers in preflight response
                .AllowAnyHeader()

                // Web Verbs like GET, POST, default enabled
                .AllowAnyMethod()

                // Refresh token header
                // Should configure here otherwise invisible for fetch response.headers
                .WithExposedHeaders(options.ExposedHeaders);
        }
    }
}
