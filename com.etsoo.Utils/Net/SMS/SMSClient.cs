using com.etsoo.Utils.Actions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.etsoo.Utils.Net.SMS
{
    /// <summary>
    /// SMS abstract client
    /// 短信抽象客户端
    /// </summary>
    public abstract class SMSClient : ISMSClient
    {
        private readonly List<SMSResource> resources = new();

        /// <summary>
        /// Add resource
        /// 添加资源
        /// </summary>
        /// <param name="resource">Resource</param>
        public void AddResource(SMSResource resource)
        {
            resources.Add(resource);
        }

        /// <summary>
        /// Get resource
        /// 获取资源
        /// </summary>
        /// <param name="country">Country</param>
        /// <param name="language">Language</param>
        /// <returns>Resource</returns>
        public SMSResource? GetResource(string? country = null, string? language = null)
        {
            IEnumerable<SMSResource> sub = resources;

            if(!string.IsNullOrEmpty(country))
            {
                sub = sub.Where(r => r.Country == null || r.Country == country).OrderByDescending(r => r.Country == country).ThenByDescending(r => r.Default);
            }

            if (!string.IsNullOrEmpty(language))
            {
                sub = sub.Where(r => r.Language == null || r.Language == language).OrderByDescending(r => r.Language == language).ThenByDescending(r => r.Default);
            }

            return sub.FirstOrDefault();
        }

        /// <summary>
        /// Async send code
        /// 异步发送验证码
        /// </summary>
        /// <returns>Result</returns>
        public abstract Task<IActionResult> SendCodeAsync(string mobile, string code, SMSResource? resource = null);
    }
}
