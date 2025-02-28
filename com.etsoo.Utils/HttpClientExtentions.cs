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
        public static void AddAuthorizationHeader(this HttpClient client, string scheme, string token)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
        }

        /// <summary>
        /// Add content language header
        /// 添加内容语言头
        /// </summary>
        /// <param name="content">HTTP content</param>
        /// <param name="contentLanguage">Content language</param>
        public static void AddContentLanguageHeader(this HttpContent content, string contentLanguage)
        {
            // com.etsoo.WebUtils\ContentLanguageHeaderRequestCultureProvider.cs to determine the culture
            // System.InvalidOperationException: Misused header name, 'Content-Language'. Make sure request headers are used with HttpRequestMessage, response headers with HttpResponseMessage, and content headers with HttpContent objects.
            content.Headers.ContentLanguage.Add(contentLanguage);
        }

        /// <summary>
        /// Add accept-language header
        /// 添加接受语言头
        /// </summary>
        /// <param name="client">HTTP client</param>
        /// <param name="acceptLanguage">Accept language</param>
        public static void AddAcceptLanguageHeader(this HttpClient client, StringValues? acceptLanguage)
        {
            if (acceptLanguage.HasValue)
            {
                foreach (var item in acceptLanguage.Value)
                {
                    client.DefaultRequestHeaders.AcceptLanguage.TryParseAdd(item);
                }
            }
        }
    }
}
