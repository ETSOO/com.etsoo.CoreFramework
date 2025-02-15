using Microsoft.Extensions.Primitives;
using System.Net.Http.Headers;

namespace com.etsoo.Utils
{
    /// <summary>
    /// HTTP client extensions
    /// HTTP 客户端扩展
    /// </summary>
    public static class HttpClientExtentions
    {
        /// <summary>
        /// Add authorization header
        /// 添加授权头
        /// </summary>
        /// <param name="client">HTTP client</param>
        /// <param name="scheme">Scheme</param>
        /// <param name="token">Token</param>
        public static void AddAuthorization(this HttpClient client, string scheme, string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
        }

        /// <summary>
        /// Add language headers
        /// 添加语言头
        /// </summary>
        /// <param name="client">HTTP client</param>
        /// <param name="acceptLanguage">Accept language</param>
        /// <param name="contentLanguage">Content language</param>
        public static void AddLanguageHeaders(this HttpClient client, StringValues? acceptLanguage, string? contentLanguage = null)
        {
            if (acceptLanguage.HasValue)
            {
                foreach (var item in acceptLanguage.Value)
                {
                    client.DefaultRequestHeaders.AcceptLanguage.TryParseAdd(item);
                }
            }

            if (contentLanguage != null)
            {
                // com.etsoo.WebUtils\ContentLanguageHeaderRequestCultureProvider.cs to determine the culture
                client.DefaultRequestHeaders.Add("Content-Language", contentLanguage);
            }
        }
    }
}
