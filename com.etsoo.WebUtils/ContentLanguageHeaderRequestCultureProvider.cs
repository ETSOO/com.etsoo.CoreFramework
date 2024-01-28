using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace com.etsoo.WebUtils
{
    /// <summary>
    /// Determines the culture information for a request via the value of the Content-Language header.
    /// </summary>
    public class ContentLanguageHeaderRequestCultureProvider : RequestCultureProvider
    {
        /// <inheritdoc />
        public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
        {
            ArgumentNullException.ThrowIfNull(httpContext);

            var contentLanguage = httpContext.Request.Headers.ContentLanguage.FirstOrDefault();

            if (string.IsNullOrEmpty(contentLanguage))
            {
                return NullProviderCultureResult;
            }

            return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(contentLanguage));
        }
    }
}
