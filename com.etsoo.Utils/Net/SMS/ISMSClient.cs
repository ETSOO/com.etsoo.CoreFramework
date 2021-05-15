using com.etsoo.Utils.Actions;
using System.Threading.Tasks;

namespace com.etsoo.Utils.Net.SMS
{
    /// <summary>
    /// SMS client interface
    /// </summary>
    public interface ISMSClient
    {
        /// <summary>
        /// Add resource
        /// 添加资源
        /// </summary>
        /// <param name="resource">Resource</param>
        void AddResource(SMSResource resource);

        /// <summary>
        /// Get resource
        /// 获取资源
        /// </summary>
        /// <param name="country">Country</param>
        /// <param name="language">Language</param>
        /// <returns>Resource</returns>
        SMSResource? GetResource(string? country = null, string? language = null);

        /// <summary>
        /// Async send code
        /// 异步发送验证码
        /// </summary>
        /// <returns>Result</returns>
        Task<IActionResult> SendCodeAsync(string mobile, string code, SMSResource? resource = null);
    }
}
