using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace com.etsoo.Utils.Localization
{
    /// <summary>
    /// Determines the culture information for a request via the value of the Content-Language header.
    /// </summary>
    public class ContentLanguageHeaderRequestCultureProvider : RequestCultureProvider
    {
        /// <inheritdoc />
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var contentLanguage = httpContext.Request.Headers["Content-Language"].FirstOrDefault();

            if (string.IsNullOrEmpty(contentLanguage))
            {
                return NullProviderCultureResult;
            }

            return Task.FromResult(new ProviderCultureResult(contentLanguage));
        }
    }
}
