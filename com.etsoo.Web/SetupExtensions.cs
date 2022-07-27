using Microsoft.AspNetCore.Cors.Infrastructure;

namespace com.etsoo.Web
{
    /// <summary>
    /// Setup extensions
    /// 设置扩展
    /// </summary>
    public static class SetupExtensions
    {
        /// <summary>
        /// Setup Cors policy
        /// </summary>
        /// <param name="builder">Builder</param>
        /// <param name="options">Options</param>
        public static void Setup(this CorsPolicyBuilder builder, CorsPolicySetupOptions options)
        {
            // .WithOrigins(Cors)
            // .SetIsOriginAllowedToAllowWildcardSubdomains()
            builder.SetIsOriginAllowed(origin => options.Check(origin))

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Access-Control-Allow-Credentials
                // https://stackoverflow.com/questions/24687313/what-exactly-does-the-access-control-allow-credentials-header-do
                // JWT is not a cookie solution, disable it without allow credential
                // .AllowCredentials()
                .DisallowCredentials()

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
